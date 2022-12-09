namespace Libmdbx.Net.Core.Env
{
    /// \brief Durability level.
    public enum Durability
    {
        RobustSynchronous,         
        HalfSynchronousWeakLast,    
        LazyWeakTail,               
        WholeFragile                
    }
}