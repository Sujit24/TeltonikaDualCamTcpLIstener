﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeltonikaDualCamTcpNetBase.Codec
{
    public static class ByteSwapper
    {
        public static short Swap(short value)
        {
            unchecked
            {
                var value1 = ((((value >> 0x08) & 0xFF) << 0x00) | (((value >> 0x00) & 0xFF) << 0x08));
                var value2 = (short)((((value >> 0x08) & 0xFF) << 0x00) | (((value >> 0x00) & 0xFF) << 0x08));
                return (short)((((value >> 0x08) & 0xFF) << 0x00) | (((value >> 0x00) & 0xFF) << 0x08));
            }
        }

        public static ushort Swap(ushort value)
        {
            unchecked
            {
                return (ushort)((((value >> 0x08) & 0xFF) << 0x00) | (((value >> 0x00) & 0xFF) << 0x08));
            }
        }

        public static int Swap(int value)
        {
            unchecked
            {
                return
                    (((value >> 0x18) & 0xFF) << 0x00) |
                    (((value >> 0x10) & 0xFF) << 0x08) |
                    (((value >> 0x08) & 0xFF) << 0x10) |
                    (((value >> 0x00) & 0xFF) << 0x18);
            }
        }

        public static uint Swap(uint value)
        {
            unchecked
            {
                return
                    (((value >> 0x18) & 0xFF) << 0x00) |
                    (((value >> 0x10) & 0xFF) << 0x08) |
                    (((value >> 0x08) & 0xFF) << 0x10) |
                    (((value >> 0x00) & 0xFF) << 0x18);
            }
        }

        public static long Swap(long value)
        {
            unchecked
            {
                return
                    (((value >> 0x38) & 0xFF) << 0x00) |
                    (((value >> 0x30) & 0xFF) << 0x08) |
                    (((value >> 0x28) & 0xFF) << 0x10) |
                    (((value >> 0x20) & 0xFF) << 0x18) |
                    (((value >> 0x18) & 0xFF) << 0x20) |
                    (((value >> 0x10) & 0xFF) << 0x28) |
                    (((value >> 0x08) & 0xFF) << 0x30) |
                    (((value >> 0x00) & 0xFF) << 0x38);
            }
        }

        public static ulong Swap(ulong value)
        {
            unchecked
            {
                return
                    (((value >> 0x38) & 0xFF) << 0x00) |
                    (((value >> 0x30) & 0xFF) << 0x08) |
                    (((value >> 0x28) & 0xFF) << 0x10) |
                    (((value >> 0x20) & 0xFF) << 0x18) |
                    (((value >> 0x18) & 0xFF) << 0x20) |
                    (((value >> 0x10) & 0xFF) << 0x28) |
                    (((value >> 0x08) & 0xFF) << 0x30) |
                    (((value >> 0x00) & 0xFF) << 0x38);
            }
        }
    }
}