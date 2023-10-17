using System.IO;

namespace Foundation {

    public static class CompressedInt {

        public static long decompress_int(BinaryReader reader) {
            byte lead = reader.ReadByte();
            var t = ~lead;
            ulong ret;
            if ((t & 0x80) != 0) {
                ret = lead;
                if ((ret & (1L << 6)) != 0) {
                    ret |= ~((1UL << 7) - 1UL);
                }
            } else if ((t & 0x40) != 0) {
                ret = reader.ReadByte();
                ret |= (ulong)(lead & 0x3F) << 8;
                if ((ret & (1UL << 13)) != 0) {
                    ret |= ~((1UL << 14) - 1UL);
                }
            } else if ((t & 0x20) != 0) {
                var tails = reader.ReadBytes(2);
                ret = tails[1];
                ret |= (ulong)tails[0] << 8;
                ret |= (ulong)(lead & 0x1F) << 16;
                if ((ret & (1L << 20)) != 0) {
                    ret |= ~((1UL << 21) - 1UL);
                }
            } else if ((t & 0x10) != 0) {
                var tails = reader.ReadBytes(3);
                ret = tails[2];
                ret |= (ulong)tails[1] << 8;
                ret |= (ulong)tails[0] << 16;
                ret |= (ulong)(lead & 0xF) << 24;
                if ((ret & (1L << 27)) != 0) {
                    ret |= ~((1UL << 28) - 1UL);
                }
            } else if ((t & 0x8) != 0) {
                var tails = reader.ReadBytes(4);
                ret = tails[3];
                ret |= (ulong)tails[2] << 8;
                ret |= (ulong)tails[1] << 16;
                ret |= (ulong)tails[0] << 24;
                ret |= (ulong)(lead & 0x7) << 32;
                if ((ret & (1UL << 34)) != 0) {
                    ret |= ~((1UL << 35) - 1UL);
                }
            } else if ((t & 0x4) != 0) {
                var tails = reader.ReadBytes(5);
                ret = tails[4];
                ret |= (ulong)tails[3] << 8;
                ret |= (ulong)tails[2] << 16;
                ret |= (ulong)tails[1] << 24;
                ret |= (ulong)tails[0] << 32;
                ret |= (ulong)(lead & 0x3) << 40;
                if ((ret & (1L << 41)) != 0) {
                    ret |= ~((1UL << 42) - 1UL);
                }
            } else if ((t & 0x2) != 0) {
                var tails = reader.ReadBytes(6);
                ret = tails[5];
                ret |= (ulong)tails[4] << 8;
                ret |= (ulong)tails[3] << 16;
                ret |= (ulong)tails[2] << 24;
                ret |= (ulong)tails[1] << 32;
                ret |= (ulong)tails[0] << 40;
                ret |= (ulong)(lead & 0x1) << 48;
                if ((ret & (1UL << 48)) != 0) {
                    ret |= ~((1UL << 49) - 1UL);
                }
            } else if ((t & 0x1) != 0) {
                var tails = reader.ReadBytes(7);
                ret = tails[6];
                ret |= (ulong)tails[5] << 8;
                ret |= (ulong)tails[4] << 16;
                ret |= (ulong)tails[3] << 24;
                ret |= (ulong)tails[2] << 32;
                ret |= (ulong)tails[1] << 40;
                ret |= (ulong)tails[0] << 48;
                if ((ret & (1UL << 55)) != 0) {
                    ret |= ~((1UL << 56) - 1UL);
                }
            } else {
                var tails = reader.ReadBytes(8);
                ret = tails[7];
                ret |= (ulong)tails[6] << 8;
                ret |= (ulong)tails[5] << 16;
                ret |= (ulong)tails[4] << 24;
                ret |= (ulong)tails[3] << 32;
                ret |= (ulong)tails[2] << 40;
                ret |= (ulong)tails[1] << 48;
                ret |= (ulong)tails[0] << 56;
            }
            return (long)ret;
        }

        public static ulong decompress_uint(BinaryReader reader) {
            var lead = reader.ReadByte();
            var t = ~lead;
            ulong ret;
            if ((t & 0x80) != 0) {
                ret = lead;
            } else if ((t & 0x40) != 0) {
                ret = reader.ReadByte();
                ret |= (ulong)(lead & 0x3F) << 8;
            } else if ((t & 0x20) != 0) {
                var tails = reader.ReadBytes(2);
                ret = tails[1];
                ret |= (ulong)tails[0] << 8;
                ret |= (ulong)(lead & 0x1F) << 16;
            } else if ((t & 0x10) != 0) {
                var tails = reader.ReadBytes(3);
                ret = tails[2];
                ret |= (ulong)tails[1] << 8;
                ret |= (ulong)tails[0] << 16;
                ret |= (ulong)(lead & 0xF) << 24;
            } else if ((t & 0x8) != 0) {
                var tails = reader.ReadBytes(4);
                ret = tails[3];
                ret |= (ulong)tails[2] << 8;
                ret |= (ulong)tails[1] << 16;
                ret |= (ulong)tails[0] << 24;
                ret |= (ulong)(lead & 0x7) << 32;
            } else if ((t & 0x4) != 0) {
                var tails = reader.ReadBytes(5);
                ret = tails[4];
                ret |= (ulong)tails[3] << 8;
                ret |= (ulong)tails[2] << 16;
                ret |= (ulong)tails[1] << 24;
                ret |= (ulong)tails[0] << 32;
                ret |= (ulong)(lead & 0x3) << 40;
            } else if ((t & 0x2) != 0) {
                var tails = reader.ReadBytes(6);
                ret = tails[5];
                ret |= (ulong)tails[4] << 8;
                ret |= (ulong)tails[3] << 16;
                ret |= (ulong)tails[2] << 24;
                ret |= (ulong)tails[1] << 32;
                ret |= (ulong)tails[0] << 40;
                ret |= (ulong)(lead & 0x1) << 48;
            } else if ((t & 0x1) != 0) {
                var tails = reader.ReadBytes(7);
                ret = tails[6];
                ret |= (ulong)tails[5] << 8;
                ret |= (ulong)tails[4] << 16;
                ret |= (ulong)tails[3] << 24;
                ret |= (ulong)tails[2] << 32;
                ret |= (ulong)tails[1] << 40;
                ret |= (ulong)tails[0] << 48;
            } else {
                var tails = reader.ReadBytes(8);
                ret = tails[7];
                ret |= (ulong)tails[6] << 8;
                ret |= (ulong)tails[5] << 16;
                ret |= (ulong)tails[4] << 24;
                ret |= (ulong)tails[3] << 32;
                ret |= (ulong)tails[2] << 40;
                ret |= (ulong)tails[1] << 48;
                ret |= (ulong)tails[0] << 56;
            }
            return ret;
        }

        public static void compress_uint(BinaryWriter writer, ulong v) {
            if (v <= 0x7FUL) {
                writer.Write((byte)v);
            } else if (v < 0x3FFFUL) {
                var data = new byte[2];
                data[0] = (byte)(0x80 | ((v >> 8) & 0x3F));
                data[1] = (byte)v;
                writer.Write(data);
            } else if (v <= 0x1FFFFFUL) {
                var data = new byte[3];
                data[0] = (byte)(0xC0 | ((v >> 16) & 0x1F));
                data[1] = (byte)(v >> 8);
                data[2] = (byte)v;
                writer.Write(data);
            } else if (v <= 0xFFFFFFFUL) {
                var data = new byte[4];
                data[0] = (byte)(0xE0 | ((v >> 24) & 0xF));
                data[1] = (byte)(v >> 16);
                data[2] = (byte)(v >> 8);
                data[3] = (byte)v;
                writer.Write(data);
            } else if (v <= 0x7FFFFFFFFUL) {
                var data = new byte[5];
                data[0] = (byte)(0xF0 | ((v >> 32) & 0x7));
                data[1] = (byte)(v >> 24);
                data[2] = (byte)(v >> 16);
                data[3] = (byte)(v >> 8);
                data[4] = (byte)v;
                writer.Write(data);
            } else if (v <= 0x3FFFFFFFFFFUL) {
                var data = new byte[6];
                data[0] = (byte)(0xF8 | ((v >> 40) & 0x3));
                data[1] = (byte)(v >> 32);
                data[2] = (byte)(v >> 24);
                data[3] = (byte)(v >> 16);
                data[4] = (byte)(v >> 8);
                data[5] = (byte)v;
                writer.Write(data);
            } else if (v <= 0x1FFFFFFFFFFFFUL) {
                var data = new byte[7];
                data[0] = (byte)(0xFC | ((v >> 48) & 0x1));
                data[1] = (byte)(v >> 40);
                data[2] = (byte)(v >> 32);
                data[3] = (byte)(v >> 24);
                data[4] = (byte)(v >> 16);
                data[5] = (byte)(v >> 8);
                data[6] = (byte)v;
                writer.Write(data);
            } else if (v <= 0xFFFFFFFFFFFFFFUL) {
                var data = new byte[8];
                data[0] = 0xFE;
                data[1] = (byte)(v >> 48);
                data[2] = (byte)(v >> 40);
                data[3] = (byte)(v >> 32);
                data[4] = (byte)(v >> 24);
                data[5] = (byte)(v >> 16);
                data[6] = (byte)(v >> 8);
                data[7] = (byte)v;
                writer.Write(data);
            } else {
                var data = new byte[9];
                data[0] = 0xFF;
                data[1] = (byte)(v >> 56);
                data[2] = (byte)(v >> 48);
                data[3] = (byte)(v >> 40);
                data[4] = (byte)(v >> 32);
                data[5] = (byte)(v >> 24);
                data[6] = (byte)(v >> 16);
                data[7] = (byte)(v >> 8);
                data[8] = (byte)v;
                writer.Write(data);
            }
        }

        public static void compress_int(BinaryWriter writer, long v) {
            if (v <= 0x3FL && v > -0x40L) {
                writer.Write((byte)(v & 0x7F));
            } else if (v <= 0x1FFFL && v >= -0x2000L) {
                var data = new byte[2];
                data[0] = (byte)(0x80 | ((v >> 8) & 0x3F));
                data[1] = (byte)v;
                writer.Write(data);
            } else if (v <= 0xFFFFFL && v >= -0x100000L) {
                var data = new byte[3];
                data[0] = (byte)(0xC0 | ((v >> 16) & 0x1F));
                data[1] = (byte)(v >> 8);
                data[2] = (byte)v;
                writer.Write(data);
            } else if (v <= 0x7FFFFFFL && v >= -0x8000000L) {
                var data = new byte[4];
                data[0] = (byte)(0xE0 | ((v >> 24) & 0xF));
                data[1] = (byte)(v >> 16);
                data[2] = (byte)(v >> 8);
                data[3] = (byte)v;
                writer.Write(data);
            } else if (v <= 0x3FFFFFFFFL && v >= -0x400000000L) {
                var data = new byte[5];
                data[0] = (byte)(0xF0 | ((v >> 32) & 0x7));
                data[1] = (byte)(v >> 24);
                data[2] = (byte)(v >> 16);
                data[3] = (byte)(v >> 8);
                data[4] = (byte)v;
                writer.Write(data);
            } else if (v <= 0x1FFFFFFFFFFL && v >= -0x20000000000L) {
                var data = new byte[6];
                data[0] = (byte)(0xF8 | ((v >> 40) & 0x3));
                data[1] = (byte)(v >> 32);
                data[2] = (byte)(v >> 24);
                data[3] = (byte)(v >> 16);
                data[4] = (byte)(v >> 8);
                data[5] = (byte)v;
                writer.Write(data);
            } else if (v <= 0xFFFFFFFFFFFFL && v >= -0x1000000000000L) {
                var data = new byte[7];
                data[0] = (byte)(0xFC | ((v >> 48) & 0x1));
                data[1] = (byte)(v >> 40);
                data[2] = (byte)(v >> 32);
                data[3] = (byte)(v >> 24);
                data[4] = (byte)(v >> 16);
                data[5] = (byte)(v >> 8);
                data[6] = (byte)v;
                writer.Write(data);
            } else if (v <= 0x7FFFFFFFFFFFFFL && v >= -0x80000000000000L) {
                var data = new byte[8];
                data[0] = 0xFE;
                data[1] = (byte)(v >> 48);
                data[2] = (byte)(v >> 40);
                data[3] = (byte)(v >> 32);
                data[4] = (byte)(v >> 24);
                data[5] = (byte)(v >> 16);
                data[6] = (byte)(v >> 8);
                data[7] = (byte)v;
                writer.Write(data);
            } else {
                var data = new byte[9];
                data[0] = 0xFF;
                data[1] = (byte)(v >> 56);
                data[2] = (byte)(v >> 48);
                data[3] = (byte)(v >> 40);
                data[4] = (byte)(v >> 32);
                data[5] = (byte)(v >> 24);
                data[6] = (byte)(v >> 16);
                data[7] = (byte)(v >> 8);
                data[8] = (byte)v;
                writer.Write(data);
            }
        }

        /*
        public static int get_tails_length(byte lead) {
            var t = ~lead;
            if ((t & 0x80) != 0) {
                return 0;
            } else if ((t & 0x40) != 0) {
                return 1;
            } else if ((t & 0x20) != 0) {
                return 2;
            } else if ((t & 0x10) != 0) {
                return 3;
            } else if ((t & 0x8) != 0) {
                return 4;
            } else if ((t & 0x4) != 0) {
                return 5;
            } else if ((t & 0x2) != 0) {
                return 6;
            } else if ((t & 0x1) != 0) {
                return 7;
            } else {
                return 8;
            }
        }

        public static long uncompress_int(byte lead, byte[] tails, int offset = 0) {
            var t = ~lead;
            ulong ret = 0;
            if ((t & 0x80) != 0) {
                ret = lead;
                if ((ret & (1L << 6)) != 0) {
                    ret |= ~((1UL << 7) - 1UL);
                }
            } else if ((t & 0x40) != 0) {
                ret = tails[offset];
                ret |= (ulong)(lead & 0x3F) << 8;
                if ((ret & (1UL << 13)) != 0) {
                    ret |= ~((1UL << 14) - 1UL);
                }
            } else if ((t & 0x20) != 0) {
                ret = tails[offset + 1];
                ret |= (ulong)tails[offset] << 8;
                ret |= (ulong)(lead & 0x1F) << 16;
                if ((ret & (1L << 20)) != 0) {
                    ret |= ~((1UL << 21) - 1UL);
                }
            } else if ((t & 0x10) != 0) {
                ret = tails[offset + 2];
                ret |= (ulong)tails[offset + 1] << 8;
                ret |= (ulong)tails[offset] << 16;
                ret |= (ulong)(lead & 0xF) << 24;
                if ((ret & (1L << 27)) != 0) {
                    ret |= ~((1UL << 28) - 1UL);
                }
            } else if ((t & 0x8) != 0) {
                ret = tails[offset + 3];
                ret |= (ulong)tails[offset + 2] << 8;
                ret |= (ulong)tails[offset + 1] << 16;
                ret |= (ulong)tails[offset] << 24;
                ret |= (ulong)(lead & 0x7) << 32;
                if ((ret & (1UL << 34)) != 0) {
                    ret |= ~((1UL << 35) - 1UL);
                }
            } else if ((t & 0x4) != 0) {
                ret = tails[offset + 4];
                ret |= (ulong)tails[offset + 3] << 8;
                ret |= (ulong)tails[offset + 2] << 16;
                ret |= (ulong)tails[offset + 1] << 24;
                ret |= (ulong)tails[offset] << 32;
                ret |= (ulong)(lead & 0x3) << 40;
                if ((ret & (1L << 41)) != 0) {
                    ret |= ~((1UL << 42) - 1UL);
                }
            } else if ((t & 0x2) != 0) {
                ret = tails[offset + 5];
                ret |= (ulong)tails[offset + 4] << 8;
                ret |= (ulong)tails[offset + 3] << 16;
                ret |= (ulong)tails[offset + 2] << 24;
                ret |= (ulong)tails[offset + 1] << 32;
                ret |= (ulong)tails[offset] << 40;
                ret |= (ulong)(lead & 0x1) << 48;
                if ((ret & (1UL << 48)) != 0) {
                    ret |= ~((1UL << 49) - 1UL);
                }
            } else if ((t & 0x1) != 0) {
                ret = tails[offset + 6];
                ret |= (ulong)tails[offset + 5] << 8;
                ret |= (ulong)tails[offset + 4] << 16;
                ret |= (ulong)tails[offset + 3] << 24;
                ret |= (ulong)tails[offset + 2] << 32;
                ret |= (ulong)tails[offset + 1] << 40;
                ret |= (ulong)tails[offset] << 48;
                if ((ret & (1UL << 55)) != 0) {
                    ret |= ~((1UL << 56) - 1UL);
                }
            } else {
                ret = tails[offset + 7];
                ret |= (ulong)tails[offset + 6] << 8;
                ret |= (ulong)tails[offset + 5] << 16;
                ret |= (ulong)tails[offset + 4] << 24;
                ret |= (ulong)tails[offset + 3] << 32;
                ret |= (ulong)tails[offset + 2] << 40;
                ret |= (ulong)tails[offset + 1] << 48;
                ret |= (ulong)tails[offset] << 56;
            }
            return (long)ret;
        }
        */
    }
}