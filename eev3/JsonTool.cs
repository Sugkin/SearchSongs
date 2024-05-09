using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eev3
{
    public static class JsonTool
    {
        /// <summary>
        /// 序列化实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(this string input)
        {
            if (input.IsEmpty()) { return default(T); }
            try
            {
                return JsonConvert.DeserializeObject<T>(input);
            }
            catch
            {
                return default(T);
            }
        }
        /// <summary>
        /// 判断是否值类型
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        public static bool IsValueTypes(this Type T)
        {
            if (T == typeof(double)
               || T == typeof(float)
               || T == typeof(decimal)
               || T == typeof(string)
               || T == typeof(char)
               || T == typeof(DateTime)
               || T == typeof(object)
               || T == typeof(bool)
               || T == typeof(long)
               || T == typeof(int)
               || T == typeof(sbyte)
               || T == typeof(short)
               || T == typeof(uint)
               || T == typeof(ulong)
               || T == typeof(ushort))
            {
                return true;
            }
            return false;
        }
        private static bool isValueTypes(this JTokenType type)
        {
            if (type == JTokenType.String
                || type == JTokenType.Integer
                || type == JTokenType.Float
                || type == JTokenType.Date
                || type == JTokenType.Boolean
                || type == JTokenType.Uri)
            {
                return true;
            }
            return false;
        }
        public static JToken ToJToken(this string json)
        {
            try
            {
                return JToken.Parse(json);
            }
            catch
            {
                return null;
            }
        }
        public static TResult GetValue<TResult>(this JToken Token)
        {
            if (Token == null) { return default(TResult); }
            try
            {
                if (typeof(TResult).IsValueTypes())
                {
                    if (Token.Type.isValueTypes())
                    {
                        return Token.Value<TResult>();
                    }
                    else
                    {
                        return (TResult)Convert.ChangeType(Token.Value<object>()?.ToString(), typeof(TResult));
                    }
                }
                else
                {
                    if (typeof(TResult) == typeof(JToken))
                    {
                        return Token.Value<TResult>();
                    }
                    else
                    {
                        var value = Token.Value<object>()?.ToString();
                        return value.DeserializeObject<TResult>();
                    }
                }
            }
            catch
            {
                return default(TResult);
            }
        }
        private static JToken FindJTokenType(this JToken Token, string key, bool ValueType = true, bool isAll = false)
        {
            try
            {
                if (Token == null || key.IsEmpty()) { return null; }
                var reader = Token.CreateReader();
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals(key))
                    {
                        var jt = Token.Root.SelectToken(reader.Path);
                        if (isAll || jt.Type == JTokenType.Null)
                        {
                            return jt;
                        }
                        else if (ValueType && jt.Type.isValueTypes())
                        {
                            return jt;
                        }
                        else if (!jt.Type.isValueTypes())
                        {
                            return jt;
                        }
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        public static JToken FindJTokenValue(this string json, string key) => json.ToJToken().FindJTokenType(key);
        /// <summary>
        /// 查询JSON字符串
        /// </summary>
        /// <param name="json"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string FindJsonValue(this string json, string key) => json?.FindJTokenValue(key).GetValue<string>();
    }
}
