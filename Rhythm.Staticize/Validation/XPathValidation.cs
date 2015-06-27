using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhythm.Staticize;

namespace Rhythm.Staticize
{
    /// <summary>
    /// 提供验证 HTML 文档中特定DOM元素的位置是否与预期的位置相符（内部使用元素的XPath进行对比，预期的位置通过定义一个参考模板获得）。
    /// </summary>
    public class XPathValidation : IValidation
    {
        Dictionary<String, String> elementXPath;

        HtmlAgilityPack.HtmlDocument truthDocument;

        /// <summary>
        /// 初始化 HtmlDocumentXPathValidation 的新实例。
        /// </summary>
        /// <param name="templateFile">参考模板文件。</param>
        /// <param name="elementIds">需要进行位置检查的网页元素Id。</param>
        public XPathValidation(String templateFile, params String[] elementIds)
        {
            if (templateFile == null)
            {
                throw new ArgumentNullException("truthDocumentFile");
            }
            if (elementIds == null)
            {
                throw new ArgumentNullException("elementIds");
            }
            this.truthDocument = new HtmlAgilityPack.HtmlDocument();
            this.truthDocument.Load(templateFile);
            Init(elementIds);
        }

        /// <summary>
        /// 初始化 HtmlDocumentXPathValidation 的新实例。
        /// </summary>
        /// <param name="templateDocument">参考模板文档对象。</param>
        /// <param name="elementIds">需要进行位置检查的网页元素Id。</param>
        public XPathValidation(HtmlAgilityPack.HtmlDocument templateDocument, params String[] elementIds)
        {
            this.truthDocument = templateDocument;
            if (templateDocument == null)
            {
                throw new ArgumentNullException("truthDocument");
            }
            Init(elementIds);
        }

        void Init(String[] elementIds)
        {
            elementXPath = new Dictionary<string, string>(elementIds.Length);
            foreach (var id in elementIds)
            {
                AddXPathCheck(id);
            }
        }

        void AddXPathCheck(String id)
        {
            var element = truthDocument.GetElementbyId(id);
            if (element == null)
            {
                return;
            }
            if (elementXPath.ContainsKey(id))
            {
                return;
            }
            elementXPath.Add(id, element.XPath);
        }

        public string Validate(HtmlAgilityPack.HtmlDocument document, HtmlStaticizeContext status)
        {
            var errorMessageBuilder = new StringBuilder();
            foreach (var id in this.elementXPath.Keys)
            {
                String truthXPath = elementXPath[id];
                var element = document.GetElementbyId(id);
                if (element == null)
                {
                    errorMessageBuilder.AppendFormat("\r\n元素 \"{0}\" 在文档中不存在。", id);
                    continue;
                }
                if (element.XPath != truthXPath)
                {
                    errorMessageBuilder.AppendFormat("\r\n元素 \"{0}\" XPath 不匹配，应为\"{1}\"，但实际为\"{2}\"。\r\n行号:{3}\r\n源HTML：\r\n{4}\r\n", id, truthXPath, element.XPath, element.Line.ToString(), element.OuterHtml);
                    continue;
                }
            }
            return errorMessageBuilder.Length == 0 ? null : errorMessageBuilder.ToString();
        }


        public string Name { get { return "页面元素XPath与模板XPath是否相符。"; } }

        public ValidationType Type { get { return ValidationType.Tag; } }


        public override string ToString() { return ((IValidation)this).Name; }

    }
}
