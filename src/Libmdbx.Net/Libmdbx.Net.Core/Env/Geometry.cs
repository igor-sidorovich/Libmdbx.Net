using System;

namespace Libmdbx.Net.Core.Env
{
    public struct Geometry
    {
        public static readonly IntPtr DefaultValue = new IntPtr(-1);

        public static readonly IntPtr MinimalValue = new IntPtr(0);

        public enum Size : long
        {
            kB = 1000,                      
            MB = kB * 1000,                
            GB = MB * 1000,                 
            TB = GB * 1000,                 
            PB = TB * 1000,                 
            EB = PB * 1000,                 
            KiB = 1024,                     
            MiB = KiB << 10,                
            GiB = MiB << 10,                
            TiB = GiB << 10,                
            PiB = TiB << 10,                
            EiB = PiB << 10,               
        }

        /// <summary>
        /// The lower bound of database Size in bytes.
        /// </summary>
        public readonly IntPtr sizeLower;

        /// <summary>
        /// The Size in bytes to Setup the database Size for now.
        /// It is recommended always pass default_value in this
        /// argument except some special cases.
        /// </summary>
        public readonly IntPtr sizeNow;

        /// <summary>
        /// The upper bound of database Size in bytes.
        /// It is recommended to avoid change upper bound while database is
        /// used by other processes or threaded (i.e. just pass default_value
        /// in this argument except absolutely necessary). Otherwise you must be
        /// ready for MDBX_UNABLE_EXTEND_MAPSIZE error(s), unexpected pauses
        /// during remapping and/or system errors like "address busy", and so on. In
        /// other words, there is no way to handle a growth of the upper bound
        /// robustly because there may be a lack of appropriate system resources
        /// (which are extremely volatile in a Multi-process Multi-threaded
        /// environment).
        /// </summary>
        public readonly IntPtr sizeUpper;

        /// <summary>
        /// The growth step in bytes, must be greater than zero to allow the
        /// database to grow.
        /// </summary>
        public readonly IntPtr growthStep;

        /// <summary>
        /// The shrink threshold in bytes, must be greater than zero to allow
        /// the database to shrink.
        /// </summary>
        public readonly IntPtr shrinkThreshold;

        /// <summary>
        /// The database page Size for new database creation
        /// or default_value otherwise.
        /// Must be power of 2 in the range between MDBX_MIN_PAGESIZE(256)
        /// and MDBX_MAX_PAGESIZE(65536).
        /// </summary>
        public readonly IntPtr pageSize;

        public Geometry(IntPtr shrinkThreshold,
                        IntPtr pageSize,
                        IntPtr growthStep,
                        IntPtr sizeLower,
                        IntPtr sizeNow,
                        IntPtr sizeUpper)
        {
            this.shrinkThreshold = shrinkThreshold;
            this.pageSize = pageSize;
            this.growthStep = growthStep;
            this.sizeLower = sizeLower;
            this.sizeNow = sizeNow;
            this.sizeUpper = sizeUpper;
        }

        public static Geometry make_fixed(IntPtr sizePtr)
        {
            var geometry = new Geometry(shrinkThreshold: DefaultValue,
                pageSize: DefaultValue,
                growthStep: DefaultValue,
                sizeLower: sizePtr,
                sizeNow: sizePtr,
                sizeUpper: sizePtr);

            return geometry;
        }

        public static Geometry make_dynamic(IntPtr lowerPtr,
            IntPtr upperPtr)
        {
            var geometry = new Geometry(shrinkThreshold:DefaultValue,
                pageSize:DefaultValue,
                growthStep:DefaultValue, 
                sizeLower:lowerPtr, 
                sizeNow:lowerPtr, 
                sizeUpper:upperPtr);

            return geometry;
        }
    }
}