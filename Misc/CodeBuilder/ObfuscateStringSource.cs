//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using static System.Runtime.InteropServices.JavaScript.JSType;

//namespace CodeBuilder
//{
//    public static class ObfuscateStringSource
//    {
//        public static string? Run(ref string data, int size, bool encode, bool reverse, Order order)
//        {
//            // Needs to fit into this 
//            // string thing = "{{R_Thing}}";
//            // could be like 
//            // string thing = Reverse(Decode("abc" + "xyz"));
//            //Order order = Order.ReverseEncode;

//            return RunSplit(ref data, size, encode, reverse, order);
//        }

//        private static string? RunSplit(ref string s, int size, bool encode, bool reverse, Order order)
//        {
//            string? split = SplitStringSource(ref s, size, encode, reverse, order);
//            if (string.IsNullOrEmpty(split))
//            {
//                Console.WriteLine("[!] Failed to Split");
//                return null;
//            }

//            return split;
//        }

//        private static string? SplitStringSource(ref string s, int size, bool encode, bool reverse, Order order)
//        {
//            if (size >= s.Length)
//            {
//                Console.WriteLine("[!] Size to big for string length");
//                return string.Empty;
//            }

//            //string commentString = $"// {s}\n";

//            if (reverse && encode)
//            {
//                HandleData(ref s, order);
//            }
//            else
//            {
//                if (reverse) s = Reverse(ref s);
//                if (encode) s = Encode(ref s);
//            }

//            var splits = SplitByLength(ref s, size);
//            for (int i = 0; i < splits.Count; i++)
//            {
//                splits[i] = splits[i].Replace("\\", "\\\\"); // Escape special char '\'
//            }

//            //Random ran = new Random();
//            //string[] varNames = new string[splits.Count];
//            //for (int i = 0; i < splits.Count; i++)
//            //{
//            //    varNames[i] = GetRanName(ran);
//            //}

//            //string nl = string.Empty;
//            //if (newline) nl = "\n";

//            StringBuilder sb = new StringBuilder();
//            //string adderVar = GetRanName(ran);
//            //string adder = $"string {adderVar} = ";

//            // string resource = "{{R_Resource}}";
//            // -> 
//            // string resource = Decode(Reverse("abc" + "xyz"));

//            for (int i = 0; i < splits.Count; i++)
//            {
//                //sb.Append($"string {varNames[i]} = \"{splits[i]}\";{nl}");
//                if (i != splits.Count - 1)
//                {
//                    sb.Append($"\"{splits[i]}\" + ");
//                    //adder += $"{varNames[i]} + ";
//                }
//                else
//                {
//                    sb.Append($"\"{splits[i]}\"");
//                    //adder += $"{varNames[i]};{nl}";
//                    //sb.Append(adder);
//                }
//            }

//            string? final = sb.ToString();

//            if (reverse && encode)
//            {
//                final = HandleOutData(final, order);
//            }
//            else
//            {
//                if (encode) final  = $"Decode({final});";
//                if (reverse) final = $"Reverse({final});";
//                //if (encode) sb.Append($"\nstring {adderVar} = Decode({adderVar})"); // Decode
//                //if (reverse) sb.Append($"\nstring {adderVar} = Reverse({adderVar})"); // Reverse
//            }

//            return final;
//        }

//        private static List<string> SplitByLength(ref string data, int length)
//        {
//            List<string> splitStrings = new List<string>();
//            StringBuilder sb = new StringBuilder();
//            int currentLength = 0;

//            for (int i = 0; i < data.Length; i++)
//            {
//                currentLength++;
//                sb.Append(data[i]);
//                if (currentLength == length)
//                {
//                    splitStrings.Add(sb.ToString());
//                    sb.Clear();
//                    currentLength = 0;
//                }
//                else if (i == data.Length - 1) // Reached End
//                {
//                    splitStrings.Add(sb.ToString());
//                    sb.Clear();
//                }
//            }

//            return splitStrings;
//        }
//        private static string Reverse(ref string s)
//        {
//            StringBuilder sb = new StringBuilder();
//            for (int i = 0; i < s.Length; i++)
//            {
//                sb.Append(s[s.Length - 1 - i]);
//            }

//            return sb.ToString();
//        }

//        private static string Encode(ref string s)
//        {
//            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(s));
//        }

//        private static void HandleData(ref string s, Order order)
//        {
//            switch (order)
//            {
//                case Order.ReverseEncode:
//                    s = Reverse(ref s);
//                    s = Encode(ref s);
//                    break;
//                case Order.EncodeReverse:
//                    s = Encode(ref s);
//                    s = Reverse(ref s);
//                    break;
//                default:
//                    break;
//            }
//        }

//        private static string? HandleOutData(string s, Order order)
//        {
//            switch (order)
//            {
//                case Order.ReverseEncode:
//                    // Reverse(Decode(s))
//                    return $"Reverse(Decode({s}));";
//                    //sb.Append($"\nstring {adderVar} = Decode({adderVar});");
//                    //sb.Append($"\nstring {adderVar} = Reverse({adderVar});");
//                case Order.EncodeReverse:
//                    // Decode(Reverse(s))
//                    return $"Decode(Reverse({s}));";
//                    //sb.Append($"\nstring {adderVar} = Reverse({adderVar});");
//                    //sb.Append($"\nstring {adderVar} = Decode({adderVar});");
//                default:
//                    return null;
//            }
//        }
//        public enum Order
//        {
//            ReverseEncode,
//            EncodeReverse
//        }
//    }
//}
