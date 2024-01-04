using System;
using System.Runtime.InteropServices;

namespace Libmdbx.Net.Bindings
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MdbxDbVal
    {
        internal unsafe byte* addr;
        internal IntPtr size;

        internal unsafe MdbxDbVal(byte* pinnedOrStackAllocBuffer, int bufferSize)
        {
            addr = pinnedOrStackAllocBuffer;
            size = (IntPtr)bufferSize;
        }

        public unsafe ReadOnlySpan<byte> AsSpan() => new ReadOnlySpan<byte>((void*)this.addr, (int)this.size);

        public byte[] CopyToNewArray() => this.AsSpan().ToArray();
    }
}
