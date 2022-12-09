namespace Libmdbx.Net.Core.Env
{
    public struct CreateParameters
    {
        public Geometry geometry;

        public ushort fileModeBits;

        public bool useSubDirectory;

        public CreateParameters(Geometry geometry = default,
                                ushort fileModeBits = 640,
                                bool useSubDirectory = false)
        {
            this.geometry = geometry;
            this.fileModeBits = fileModeBits;
            this.useSubDirectory = useSubDirectory;
        }
    }
}