namespace Libmdbx.Net.Bindings
{
    public static class Const
    {
#if IOS || __IOS__
        public const string LibMdbxName = "@rpath/libmdbx.framework/libmdbx";
#else
        public const string LibMdbxName = "mdbx";

#endif

        #region Errors

        #region Windows Codes

        /// <summary>
        /// Reached the end of the file.
        /// </summary>
        public const int ERROR_HANDLE_EOF = 38;

        /// <summary>
        /// The parameter is incorrect.
        /// </summary>
        public const int ERROR_INVALID_PARAMETER = 87;

        /// <summary>
        /// Access is denied.
        /// </summary>
        public const int ERROR_ACCESS_DENIED = 5;

        /// <summary>
        /// Not enough memory resources are available to complete this operation.
        /// </summary>
        public const int ERROR_OUTOFMEMORY = 14;

        /// <summary>
        /// The specified file is read only.
        /// </summary>
        public const int ERROR_FILE_READ_ONLY = 6009;

        /// <summary>
        /// The requested action is not supported on standard server.
        /// </summary>
        public const int ERROR_NOT_SUPPORTED = 50;

        /// <summary>
        /// The system cannot write to the specified device.
        /// </summary>
        public const int ERROR_WRITE_FAULT = 29;

        /// <summary>
        /// Incorrect function.
        /// </summary>
        public const int ERROR_INVALID_FUNCTION = 1;

        /// <summary>
        /// The operation was canceled by the user.
        /// </summary>
        public const int ERROR_CANCELLED = 1223;

        /// <summary>
        /// The system cannot find the file specified.
        /// </summary>
        public const int ERROR_FILE_NOT_FOUND = 2;

        /// <summary>
        /// The remote storage service encountered a media error
        /// </summary>
        public const int ERROR_REMOTE_STORAGE_MEDIA_ERROR = 4352;

        #endregion

        #region POSIX Codes

#if !RC_INVOKED

        /// <summary>
        /// Invalid argument
        /// </summary>
        public const int EINVAL = 22;

#endif

        /// <summary>
        /// Operation not permitted
        /// </summary>
        public const int EPERM = 1;

        /// <summary>
        /// No such file or directory
        /// </summary>
        public const int ENOENT = 2;

        /// <summary>
        /// Interrupted system call
        /// </summary>
        public const int EINTR = 4;

        /// <summary>
        /// Input/output error
        /// </summary>
        public const int EIO = 5;

        /// <summary>
        /// Cannot allocate memory 
        /// </summary>
        public const int ENOMEM = 12;

        /// <summary>
        /// Permission denied
        /// </summary>
        public const int EACCES = 13;

        /// <summary>
        /// Block device required
        /// </summary>
        public const int ENOTBLK = 15;

        /// <summary>
        /// Read-only filesystem
        /// </summary>
        public const int EROFS = 30;

        /// <summary>
        /// Function not implemented
        /// </summary>
        public const int ENOSYS = 40;

        public const int EPIPE = 32;

        #endregion

#if WINDOWS || WINDOWS_UWP

        public const int MDBX_ENODATA = ERROR_HANDLE_EOF;

        public const int MDBX_EINVAL = ERROR_INVALID_PARAMETER;

        public const int MDBX_EACCESS = ERROR_ACCESS_DENIED;

        public const int MDBX_ENOMEM = ERROR_OUTOFMEMORY;

        public const int MDBX_EROFS = ERROR_FILE_READ_ONLY;

        public const int MDBX_ENOSYS = ERROR_NOT_SUPPORTED;

        public const int MDBX_EIO = ERROR_WRITE_FAULT;
    
        public const int MDBX_EPERM = ERROR_INVALID_FUNCTION;

        public const int MDBX_EINTR = ERROR_CANCELLED;

        public const int MDBX_ENOFILE = ERROR_FILE_NOT_FOUND;

        public const int MDBX_EREMOTE = ERROR_REMOTE_STORAGE_MEDIA_ERROR;

#elif OSX || IOS || __IOS__

        public const int MDBX_ENODATA = 96;

        public const int MDBX_EINVAL = EINVAL;

        public const int MDBX_EACCESS = EACCES;

        public const int MDBX_ENOMEM = ENOMEM; 

        public const int MDBX_EROFS = EROFS;

        public const int MDBX_ENOSYS = ENOSYS;

        public const int MDBX_EIO = EIO;

        public const int MDBX_EPERM = EPERM;

        public const int MDBX_EINTR = EINTR;

        public const int MDBX_ENOFILE = ENOENT;

        public const int MDBX_EREMOTE = ENOTBLK;

#elif LINUX || ANDROID

    #if ENODATA

        public const int MDBX_ENODATA = ENODATA;

    #else

        public const int MDBX_ENODATA = 9919 /* for compatibility with LLVM's C++ libraries/headers */;

    #endif
        
        public const int MDBX_EINVAL = EINVAL;

        public const int MDBX_EACCESS = EACCES;

        public const int MDBX_ENOMEM = ENOMEM; 

        public const int MDBX_EROFS = EROFS;

        public const int MDBX_ENOSYS = ENOSYS;

        public const int MDBX_EIO = EIO;

        public const int MDBX_EPERM = EPERM;

        public const int MDBX_EINTR = EINTR;

        public const int MDBX_ENOFILE = ENOENT;

        public const int MDBX_EREMOTE = ENOTBLK;

#endif

        #endregion
    }
}
