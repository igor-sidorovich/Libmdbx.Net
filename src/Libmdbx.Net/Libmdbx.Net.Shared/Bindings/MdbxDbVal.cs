using System;
using System.Runtime.InteropServices;

namespace Libmdbx.Net.Bindings
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MdbxDbVal
    {
        public IntPtr addr;
        public IntPtr size;

        public int Length => size.ToInt32();

        internal MdbxDbVal(IntPtr addr, int size)
        {
            this.addr = addr;
            this.size = IntPtr.Add(IntPtr.Zero, size);
        }
    }
}
