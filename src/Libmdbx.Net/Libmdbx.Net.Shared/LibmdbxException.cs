using System;

namespace Libmdbx.Net
{
    public class LibmdbxException : Exception
    {
        public LibmdbxResultCodeFlag ErrorCode { get; }

        public LibmdbxException(string method, LibmdbxResultCodeFlag errorCode) : base(BuildMessage(method, errorCode))
        {
            ErrorCode = errorCode;
        }

        public LibmdbxException(string method, LibmdbxResultCodeFlag errorCode, string message) : base(BuildMessage(method, errorCode, message))
        {
            ErrorCode = errorCode;
        }

        private static string BuildMessage(string method, LibmdbxResultCodeFlag errorCode)
        {
            return $"Libmdbx {method} method failed with {errorCode}:{LibmdbxResultCode.ResultCodeToString(errorCode)}";
        }

        private static string BuildMessage(string method, LibmdbxResultCodeFlag errorCode, string message)
        {
            return $"Libmdbx {method} method failed with {errorCode}:{message}";
        }
    }
}
