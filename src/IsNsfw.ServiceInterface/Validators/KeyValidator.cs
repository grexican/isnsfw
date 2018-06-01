using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace IsNsfw.ServiceInterface.Validators
{
    public class KeyValidator
    {
        public static readonly Regex TagRegex = new Regex(@"^[a-zA-Z0-9]{3,20}$", RegexOptions.Compiled);
        public static readonly string[] BannedTags = new []{ "api", "login", "logout", "account", "home", "index", "about", "aboutus", "contact", "tos", "faq" };
        private static readonly Random _random = new Random();

        public static bool ValidateKey(string key)
        {
            return TagRegex.IsMatch(key)
                   && !BannedTags.Contains(key, StringComparer.OrdinalIgnoreCase);
        }

        public static string GenerateKey(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            return new string(Enumerable.Range(1, length).Select(_ => chars[_random.Next(chars.Length)]).ToArray());
        }
    }
}
