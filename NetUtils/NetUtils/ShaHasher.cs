using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Net3Migrations.System.Web;

namespace NetUtils
{
    public class ShaHasher
    {
        public static string GetSha512ForPasswordWithSalt(string password, string salt)
        {
            using (SHA512 shaM = new SHA512Managed())
            {
                var hash = shaM.ComputeHash(Encoding.UTF8.GetBytes(password + HttpUtility.HtmlDecode(salt)));
                return ByteArrayToString(hash);
            }
        }
        public static string GenerateSalt(int length = 10)
        {
            const string chars = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return HttpUtility.HtmlEncode(new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray())); ;
        }


        static string ByteArrayToString(byte[] ba)
        {
            var hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }
    }
}