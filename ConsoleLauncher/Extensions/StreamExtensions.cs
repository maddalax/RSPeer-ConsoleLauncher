using System;
using System.IO;
using System.Security.Cryptography;

namespace ConsoleLauncher.Extensions
{
    public static class StreamExtensions
    {
        public static string CalculateHash(this Stream stream)
        {
            var provider = new SHA512CryptoServiceProvider();
            var hashedBytes = provider.ComputeHash(stream);
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }
}