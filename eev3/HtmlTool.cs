using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;

namespace eev3
{
    public enum XPathType
    {
        StartsWith,
        EndsWith,
        Contains,
        Equals,
        GreaterThan,
        LessThan,
        EqualThan,
        NotEqualThan
    }
    public static class HtmlTool
    {

        #region 查询文本字符匹配
        public static List<string> FindHtmlAttValues(this string html, string tag, string attName)
            => html?.ToHtmlDocument()?.FindNodeTags(tag)?.Select(a => a.GetAttributeValue(attName, null))?.ToList() ?? new List<string>();
        public static string FindHtmlAttValue(this string html, string tag, string attName)
            => html?.FindHtmlAttValues(tag, attName)?.Find(a => !a.IsEmpty());
        public static List<string> FindHtmlObjects(this string html, string tag)
               => html?.ToHtmlDocument()?.FindNodeTags(tag)?.Select(a => a.InnerHtml)?.ToList() ?? new List<string>();
        public static string FindHtmlObject(this string html, string tag)
                      => html?.ToHtmlDocument()?.FindNodeTag(tag)?.InnerHtml;
        public static string FindHtmlObject(this string html, string tag, string key, string value)
             => html?.ToHtmlDocument()?.FindNode(XPathType.Equals, tag, key, value)?.InnerHtml;

        #endregion

        #region  HTML节点查询

        private static string FormatXpathType(IXPathNavigable IXPath, XPathType PathType, string Tag, string Key, string Value)
        {
            string condition = null;
            //开始包含
            if (PathType == XPathType.StartsWith)
            {
                condition = $"starts-with({Key},'{Value}')";
            }
            //结束包含
            else if (PathType == XPathType.EndsWith)
            {

                condition = $"ends-with({Key},'{Value}')";
            }
            //包含
            else if (PathType == XPathType.Contains)
            {
                condition = $"contains({Key},'{Value}')";
            }
            //匹配等于
            else if (PathType == XPathType.Equals)
            {
                condition = $"{Key}='{Value}'";
            }
            //大于
            else if (PathType == XPathType.GreaterThan)
            {
                condition = $"{Key}>{Value}";
            }
            //小于
            else if (PathType == XPathType.LessThan)
            {
                condition = $"{Key}<{Value}";
            }
            //等于
            else if (PathType == XPathType.EqualThan)
            {
                condition = $"{Key}={Value}";
            }
            //不等于
            else if (PathType == XPathType.NotEqualThan)
            {
                condition = $"{Key}!={Value}";
            }
            string split = IXPath is HtmlAgilityPack.HtmlDocument ? "//" : ".//";
            var Xpath = $"{split}{Tag}[{condition}]";
            return Xpath;
        }
        public static HtmlAgilityPack.HtmlDocument ToHtmlDocument(this string html)
        {
            try
            {
                if (html.IsEmpty()) { return null; }
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);
                return doc;
            }
            catch
            {
                return null;
            }
        }
        public static HtmlAgilityPack.HtmlNode FindNode(this IXPathNavigable IXPath, string xpath)
        {
            try
            {
                if (IXPath == null || xpath.IsEmpty()) { return null; }
                var node = IXPath is HtmlAgilityPack.HtmlDocument doc ? doc.DocumentNode : IXPath as HtmlAgilityPack.HtmlNode;
                if (node == null) { return null; }
                //System.Console.WriteLine(xpath);
                return node.SelectSingleNode(xpath);
            }
            catch
            {
                return null;
            }
        }
        public static List<HtmlAgilityPack.HtmlNode> FindNodes(this IXPathNavigable IXPath, string xpath)
        {
            try
            {
                if (IXPath == null || xpath.IsEmpty()) { return null; }
                var node = IXPath is HtmlAgilityPack.HtmlDocument doc ? doc.DocumentNode : IXPath as HtmlAgilityPack.HtmlNode;
                if (node == null) { return null; }
                return node.SelectNodes(xpath)?.ToList();
            }
            catch
            {
                return null;
            }
        }
        public static HtmlAgilityPack.HtmlNode FindNodeTag(this IXPathNavigable IXPath, string Tag)
        {
            var split = IXPath is HtmlAgilityPack.HtmlDocument ? "//" : ".//";
            return IXPath.FindNode($"{split}{Tag}");
        }
        public static List<HtmlAgilityPack.HtmlNode> FindNodeTags(this IXPathNavigable IXPath, string Tag)
        {
            var split = IXPath is HtmlAgilityPack.HtmlDocument ? "//" : ".//";
            return IXPath.FindNodes($"{split}{Tag}");
        }
        public static HtmlAgilityPack.HtmlNode FindNode(this IXPathNavigable IXPath, XPathType PathType, string Tag, string Key, string Value)
            => IXPath.FindNode(FormatXpathType(IXPath, PathType, Tag, "@" + Key, Value));
        #endregion

    }
}
