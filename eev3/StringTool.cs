using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace eev3
{
    public static class StringTool
    {
        public static string RegexExtract(this string input, string pattern)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            if (string.IsNullOrEmpty(pattern)) pattern = string.Empty;
            try
            {
                Match match = Regex.Match(input, pattern);
                if (!match.Success) return string.Empty;
                return match.Groups[1].Value;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 判断是否为空
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsEmpty(this string input)
        {
            return string.IsNullOrEmpty(input);
        }
        /// <summary>
        /// 字符串分割
        /// </summary>
        /// <param name="input"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public static string[] SplitString(this string input, string split)
        {
            if (input == null) { return new string[] { }; }
            return input.Split(new string[] { split }, StringSplitOptions.None);
        }
        /// <summary>
        /// 包含多个字符串 包含其中一个为True
        /// </summary>
        /// <param name="input"></param>
        /// <param name="strs"></param>
        /// <returns></returns>
        public static bool ContainsStrings(this string input, params string[] strs)
        {
            if (input.IsEmpty()) { return false; }
            foreach (var item in strs)
            {
                if (!item.IsEmpty() && input.Contains(item))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// url编码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToUrlEncode(this string input)
        {
            return HttpUtility.UrlEncode(input);
        }
    }
}
