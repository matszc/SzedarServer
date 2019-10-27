using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace szedarserver.Infrastructure.Extensions
{
    public static class HashExtension
    {
        private static MD5 md5 = MD5.Create();
        public static string HashPassword(string password)
        {
            byte[] bHash = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(bHash).Replace("-", "").ToLower();
        }
    }
}
