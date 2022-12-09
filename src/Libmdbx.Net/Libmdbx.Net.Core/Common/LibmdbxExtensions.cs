using System.ComponentModel;
using System.Runtime.CompilerServices;
using Libmdbx.Net.Core.Env;

namespace Libmdbx.Net.Core.Common
{
    internal static class LibmdbxExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EnvFlags Mode2Flags(this Mode mode)
        {
            switch (mode)
            {
                default:
                    throw new InvalidEnumArgumentException("db::Mode is invalid");
                case Mode.ReadonlyMode:
                    return EnvFlags.ReadOnly;
                case Mode.WriteFileIo:
                    return EnvFlags.Defaults;
                case Mode.WriteMappedIo:
                    return EnvFlags.WriteMap;
            }
        }
    }
}
