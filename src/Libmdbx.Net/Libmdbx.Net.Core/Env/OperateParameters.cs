using System.ComponentModel;
using Libmdbx.Net.Core.Common;

namespace Libmdbx.Net.Core.Env
{
    public struct OperateParameters
    {
        /// <summary>
        /// The maximum number of named databases for the environment.
        /// Zero means default value.
        /// </summary>
        public uint maxMaps;

        /// <summary>
        /// The maximum number of threads/reader slots for the environment.
        /// Zero means default value.
        /// </summary>
        public uint maxReaders;

        public Mode mode;

        public Durability durability;

        public ReclaimingOptions reclaiming;

        public OperateOptions options;

        public OperateParameters(uint maxMaps = 0,
            uint maxReaders = 0,
            Mode mode = Mode.WriteMappedIo,
            Durability durability = Durability.RobustSynchronous,
            ReclaimingOptions reclaiming = default,
            OperateOptions options = default)
        {
            this.maxMaps = maxMaps;
            this.maxReaders = maxReaders;
            this.mode = mode;
            this.durability = durability;
            this.reclaiming = reclaiming;
            this.options = options;
        }

        public OperateParameters(IEnv env)
        {
            maxMaps = env.MaxMaps();
            maxReaders = env.MaxReaders();
            var flags = env.GetFlags();
            mode = ModeFromFlags(flags);
            durability = DurabilityFromFlags(flags);
            reclaiming = ReclaimingFromFlags(flags);
            options = OptionsFromFlags(flags);
        }

        public EnvFlags MakeFlags(bool accede = true, bool useSubDirectory = false)
        {
            EnvFlags flagsT = mode.Mode2Flags();
            if (accede)
                flagsT |= EnvFlags.Accede;
            if (!useSubDirectory)
                flagsT |= EnvFlags.NoSubDir;
            if (options.exclusive)
                flagsT |= EnvFlags.Exclusive;
            if (options.orphanReadTransactions)
                flagsT |= EnvFlags.NoTls;
            if (options.disableReadahead)
                flagsT |= EnvFlags.NordAhead;
            if (options.disableClearMemory)
                flagsT |= EnvFlags.NoMemInit;

            if (mode != Mode.ReadonlyMode)
            {
                if (options.nestedWriteTransactions)
                    flagsT &= ~EnvFlags.WriteMap;
                if (reclaiming.coalesce)
                    flagsT |= EnvFlags.Coalesce;
                if (reclaiming.lifo)
                    flagsT |= EnvFlags.LifoReclaim;
                switch (durability)
                {
                    default:
                        throw new InvalidEnumArgumentException("db::Durability is invalid");
                    case Durability.RobustSynchronous:
                        break;
                    case Durability.HalfSynchronousWeakLast:
                        flagsT |= EnvFlags.NoMetaSync;
                        break;
                    case Durability.LazyWeakTail:
                        flagsT |= EnvFlags.SafeNoSync;
                        break;
                    case Durability.WholeFragile:
                        flagsT |= EnvFlags.UtterlyNoSync;
                        break;
                }
            }
            return flagsT;
        }

        public static Mode ModeFromFlags(EnvFlags flagsT)
        {
            if ((flagsT & EnvFlags.ReadOnly) != 0)
                return Mode.ReadonlyMode;

            return (flagsT & EnvFlags.WriteMap) != 0 ? Mode.WriteMappedIo
                : Mode.WriteFileIo;
        }

        public static Durability DurabilityFromFlags(EnvFlags flagsT)
        {
            if ((flagsT & EnvFlags.UtterlyNoSync) == EnvFlags.UtterlyNoSync)
                return Durability.WholeFragile;

            if ((flagsT & EnvFlags.SafeNoSync) != 0)
                return Durability.LazyWeakTail;

            if ((flagsT & EnvFlags.NoMetaSync) != 0)
                return Durability.HalfSynchronousWeakLast;

            return Durability.RobustSynchronous;
        }

        public static ReclaimingOptions ReclaimingFromFlags(EnvFlags flags)
        {
            return new ReclaimingOptions(flags);
        }

        public static OperateOptions OptionsFromFlags(EnvFlags flags)
        {
            return new OperateOptions(flags);
        }
    }
}