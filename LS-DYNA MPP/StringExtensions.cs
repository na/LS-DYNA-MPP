using System;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Predictive.StringExtensions
{
    public static class StringExtensions
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flag">The flag to use to</param>
        /// <returns>String</returns>
        public static string ToWords(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }
            else
            {
                int wordSize = 0;
                wordSize = System.Environment.Is64BitOperatingSystem ? 8 : 4;

                var words = str.Trim();
                words = words.ToLower();
                
                string pattern = "(kib|mib|gib|kb|mb|gb|words)";

                string[] substrings = Regex.Split(words, pattern);

                if (substrings.Length == 1)
                {
                    words = str;
                }
                else
                {

                    long value = 0;
                    Int64.TryParse(substrings[0], out value);

                    string size = substrings[1];

                    switch (size)
                    {
                        case "kb":
                            value = value * 1000 / wordSize;
                            words = value.ToString();
                            break;
                        case "mb":
                            value = value * 1000 * 1000 / wordSize;
                            words = value.ToString();
                            break;
                        case "gb":
                            value = value * 1000 * 1000 * 1000 / wordSize;
                            words = value.ToString();
                            break;
                        case "kib":
                            value = value * 1024 / wordSize;
                            words = value.ToString();
                            break;
                        case "mib":
                            value = value * 1024 * 1024 / wordSize;
                            words = value.ToString();
                            break;
                        case "gib":
                            value = value * 1024 * 1024 * 1024 / wordSize;
                            words = value.ToString();
                            break;
                        default:
                            words = value.ToString();
                            break;
                    }
                }
                return words;
            }
        }
        /// <summary>
        /// Uses System.IO.Path.GetDirectory to get the directory of a string.
        /// </summary>
        /// <returns>string</returns>
        public static string Directory(this String str)
        {
            //Debug.WriteLine("String: " + str);
            return string.IsNullOrWhiteSpace(str) ? str : Path.GetDirectoryName(str);
            //return "Hello World";
        }

        /// <summary>
        /// Uses System.IO.Path.GetFileNameWithoutExtension to get the filename without extension of a string.
        /// </summary>
        /// <returns>string</returns>
        public static string FileNameWithoutExtension(this String str)
        {
            return string.IsNullOrWhiteSpace(str) ? str : Path.GetFileNameWithoutExtension(str);
        }

        /// <summary>
        /// Given a flag ToOption returns flag=string or 'flag=string with space' if string contains spaces.
        /// </summary>
        /// <param name="flag">The flag to use to</param>
        /// <returns>string</returns>
        public static string ToOption(this string str, string flag)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            } else {
                return string.Format("{0}{1}", flag, str);
            }
        }

        public static string GetShortPathName(this string str)
        {
            if (!String.IsNullOrEmpty(str))
            {
                StringBuilder shortPath = new StringBuilder(str.Length + 1);
                if (0 == GetShortPathName(str, shortPath, shortPath.Capacity))
                {
                    return str;
                } 
                return shortPath.ToString();
            }
            return "";
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern Int32 GetShortPathName(String path, StringBuilder shortPath, Int32 shortPathLength);

        public static string GetLongPathName(this string str)
        {
            if (!String.IsNullOrEmpty(str))
            {
                StringBuilder longPath = new StringBuilder(str.Length + 1);
                if (0 == GetLongPathName(str, longPath, longPath.Capacity))
                {
                    return str;
                }
                return longPath.ToString();
            }
            return "";
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern Int32 GetLongPathName(String path, StringBuilder shortPath, Int32 shortPathLength);
    }
}
