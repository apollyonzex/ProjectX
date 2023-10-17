
using System;
using System.IO;

namespace Foundation.Packets {
    public interface IPacketData {
        bool validate(int[] size_hint, int offset);
        void save_to(BinaryWriter w);
    }

    public interface IPacketEnumBase {

    }

    public interface IPacketReader {
        BinaryReader reader { get; }
        void notify_read();
        void post(Action<object> action);
    }

    public interface IPacket : IPacketData {
        uint pid { get; }
        void post_process(IPacketReader reader);
    }

}