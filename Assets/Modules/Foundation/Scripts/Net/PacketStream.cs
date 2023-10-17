
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Foundation.Packets;

namespace Foundation.Net {

    public class PacketStream : IDisposable {

        public event Action on_connected {
            add => m_stream.on_connected += value;
            remove => m_stream.on_connected -= value;
        }

        public event Action on_connect_failed {
            add => m_stream.on_connect_failed += value;
            remove => m_stream.on_connect_failed -= value;
        }

        public event Action on_disconnected {
            add => m_stream.on_disconnected += value;
            remove => m_stream.on_disconnected -= value;
        }

        public event Action<ulong> on_receive_invalid_pid;

        public PacketStream(string host, int port, object handler) {
            m_last_received = m_last_sent = Environment.TickCount;
            m_handler = handler;
            m_registry = get_registry(handler.GetType());
            m_stream = new TcpStream(host, port, receive);
        }

        public void replace_handler(object handler) {
            var registry = get_registry(handler.GetType());
            lock (m_handler_lock) {
                m_handler = handler;
                m_registry = registry;
            }
        }

        public void close() {
            GC.SuppressFinalize(this);
            m_stream.close();
        }

        public void update() {
            m_stream.update();
        }

        public void post_send(IPacket pkt, bool no_response = false) {
            m_stream.post_send((w, p) => {
                w.write_compressed_uint(pkt.pid);
                pkt.save_to(w);
                w.Flush();
                if (!no_response) {
                    m_last_sent = Environment.TickCount;
                }
            });
        }

        public bool connected => m_stream.connected;

        public int milliseconds_since_last_received => Environment.TickCount - m_last_received;
        public int milliseconds_since_last_sent => Environment.TickCount - m_last_sent;

        ~PacketStream() {
            m_stream.close();
        }

        void IDisposable.Dispose() {
            GC.SuppressFinalize(this);
            m_stream.close();
        }

        private class Reader : IPacketReader {
            public PacketStream stream { get; }
            public BinaryReader reader { get; }
            public Action<Action> poster { get; }
            public object handler { get; set; }

            public Reader(PacketStream stream, BinaryReader reader, Action<Action> poster) {
                this.stream = stream;
                this.reader = reader;
                this.poster = poster;
            }

            public void post(Action<object> action) {
                var handler = this.handler;
                poster(() => action(handler));
            }

            public void notify_read() {
                stream.m_last_received = Environment.TickCount;
            }
        }

        private void receive(BinaryReader r, Action<Action> p) {
            var ctor_param = new object[] { r };
            var reader = new Reader(this, r, p);
            for (;;) {
                var pid = r.read_compressed_uint();
                object handler;
                Dictionary<uint, PacketInfo> registry;
                lock (m_handler_lock) {
                    handler = m_handler;
                    registry = m_registry;
                }
                if (pid > 0xFFFFFFFF || !m_registry.TryGetValue((uint)pid, out var info)) {
                    p(() => on_receive_invalid_pid?.Invoke(pid));
                    break;
                }
                var pkt = (IPacket)info.ctor.Invoke(ctor_param);
                m_last_received = Environment.TickCount;
                p(() => info.proc.Invoke(handler, new object[] { pkt }));
                reader.handler = handler;
                pkt.post_process(reader);
            }
        }


        private TcpStream m_stream;
        private object m_handler_lock = new object();
        private object m_handler;
        private Dictionary<uint, PacketInfo> m_registry;
        private volatile int m_last_received;
        private volatile int m_last_sent;
        

        class PacketInfo {
            public ConstructorInfo ctor;
            public MethodInfo proc;
        }

        static Dictionary<Type, Dictionary<uint, PacketInfo>> s_registries = new Dictionary<Type, Dictionary<uint, PacketInfo>>();

        static Dictionary<uint, PacketInfo> get_registry(Type ty) {
            if (s_registries.TryGetValue(ty, out var ret)) {
                return ret;
            }

            var pkt_ty = typeof(IPacket);
            var pid_flags = BindingFlags.Static | BindingFlags.Public;
            var ctor_param = new Type[] { typeof(BinaryReader) };

            ret = new Dictionary<uint, PacketInfo>();
            foreach (var mi in ty.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
                if (mi.Name != "process") {
                    continue;
                }
                var ps = mi.GetParameters();
                if (ps.Length != 1) {
                    continue;
                }
                var p = ps[0];
                if (!pkt_ty.IsAssignableFrom(p.ParameterType)) {
                    continue;
                }
                var id = p.ParameterType.GetField("PID", pid_flags);
                if (id == null || id.FieldType != typeof(uint)) {
                    continue;
                }
                var ctor = p.ParameterType.GetConstructor(ctor_param);
                if (ctor == null) {
                    continue;
                }
                ret.Add((uint)id.GetValue(null), new PacketInfo {
                    ctor = ctor,
                    proc = mi,
                });
            }
            s_registries.Add(ty, ret);
            return ret;
        }
    }

}