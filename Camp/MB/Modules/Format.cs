
using System;

namespace MB1.Modules
{
    public static class Format
    {
        public static byte[] DecodeBase64(string s)
        {         
            return Convert.FromBase64String(s);
        }
    }
}
