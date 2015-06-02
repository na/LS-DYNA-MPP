using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Predictive.StringExtensions
{
    public static class StringExtensions
    {
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
            //Debug.WriteLine("String: " + str);
            return string.IsNullOrWhiteSpace(str) ? str : Path.GetFileNameWithoutExtension(str);
            //return "Hello World";
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
