
using System;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.IO;

namespace Foundation.Net {

    public class TcpStream : IDisposable {

        public event Action on_connected;
        public event Action on_connect_failed;
        public event Action on_disconnected;

        public delegate void Receive(BinaryReader reader, Action<Action> post);
        public delegate void Send(BinaryWriter writer, Action<Action> post);

        public TcpStream(string host, int port, Receive receive) {
            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_socket.NoDelay = true;
            m_receive = receive;
            m_sending = new Thread(send);
            m_receiving = new Thread(() => connect_and_receive(host, port));
            m_receiving.Start();
        }

        public bool connected { get; private set; }

        public void update() {
            lock (m_msg_queue_lock) {
                var t = m_msg_queue_out;
                m_msg_queue_out = m_msg_queue_in;
                m_msg_queue_in = t;
            }
            while (m_msg_queue_out.Count != 0) {
                m_msg_queue_out.Dequeue()();
                if (m_destroyed) {
                    break;
                }
            }
        }

        public void close() {
            GC.SuppressFinalize(this);
            destroy();
        }

        public void post_send(Send action) {
            lock (m_send_queue) {
                m_send_queue.Enqueue(action);
            }
            m_send_semaphore.Release();
        }

        void IDisposable.Dispose() {
            close();
        }

        ~TcpStream() {
            destroy();
        }

        private bool m_destroyed = false;

        private void connect_and_receive(string host, int port) {
            try {
                m_socket.Connect(host, port);
            } catch (Exception) {
                post_msg(() => on_connect_failed?.Invoke());
                return;
            }

            post_msg(() => { connected = true; on_connected?.Invoke(); });
            m_sending.Start();
            try {
                var stream = new BufferedStream(new NetworkStream(m_socket), 8192);
                m_receive?.Invoke(new BinaryReader(stream), post_msg);
                m_socket.Shutdown(SocketShutdown.Both);
            } catch (Exception) {

            }
            
            post_msg(() => { connected = false; on_disconnected?.Invoke(); });
        }

        private void send() {
            var stream = new BufferedStream(new NetworkStream(m_socket));
            var writer = new BinaryWriter(stream);
            for (;;) {
                m_send_semaphore.Wait();
                Send action;
                lock (m_send_queue) {
                    action = m_send_queue.Dequeue();
                }
                try {
                    action(writer, post_msg);
                } catch (Exception) {

                }
            }
        }

        private void post_msg(Action action) {
            lock (m_msg_queue_lock) {
                m_msg_queue_in.Enqueue(action);
            }
        }

        private void destroy() {
            m_destroyed = true;
            m_sending.Abort();
            m_receiving.Abort();
            m_socket.Dispose();
        }

        protected Socket m_socket;
        private Thread m_receiving;
        private Thread m_sending;
        private Receive m_receive;

        private Queue<Action> m_msg_queue_in = new Queue<Action>();
        private Queue<Action> m_msg_queue_out = new Queue<Action>();
        private object m_msg_queue_lock = new object();
        private Queue<Send> m_send_queue = new Queue<Send>();
        private SemaphoreSlim m_send_semaphore = new SemaphoreSlim(0);

    }

}