using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rhythm.Staticize
{
    /// <summary>
    /// 提供对常用 HTML 验证方法的支持。
    /// </summary>
    public static class ValidationProjection
    {
        /// <summary>
        /// 验证 HTML 中是否包含指定内容。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static IValidation Contains(String value, String errorMessage = null)
        {
            return new ValidationDelegateTaken("页面是否存在指定内容", ValidationType.Content, (doc) =>
            {
                var htmlNode = doc.DocumentNode;
                return htmlNode.InnerHtml.Contains(value);
            }, errorMessage ?? string.Format("页面中不存在预期的内容：\"{0}\"。", value));
        }

        /// <summary>
        /// 验证 HTML 网页标题是否等于预期的标题。
        /// </summary>
        /// <param name="excepted">预期的标题</param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static IValidation TitleEquals(String excepted, String errorMessage = null)
        {
            return new ValidationDelegateTaken("网页标题", ValidationType.Content, (doc) =>
            {
                var htmlNode = doc.DocumentNode;
                var titleNode = htmlNode.SelectSingleNode(@"html/head/title");
                if (titleNode == null)
                {
                    return false;
                }
                return excepted == titleNode.InnerHtml;
            }, errorMessage ?? string.Format("预期的标题 \"{0}\"。", excepted));
        }

        /// <summary>
        /// 验证 HTML DOM 中是否存在指定的元素。
        /// </summary>
        /// <param name="elementId"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static IValidation HasElement(String elementId, String errorMessage = null)
        {
            return new ValidationDelegateTaken("页面元素是否存在", ValidationType.Tag, (doc) =>
            {
                var htmlNode = doc.DocumentNode;
                var element = doc.GetElementbyId(elementId);
                return element != null;
            }, errorMessage ?? string.Format("页面中不存在元素\"{0}\"。", elementId));
        }

        /// <summary>
        /// 验证 HTML Docuemnt 是否包含指定的 CSS 文件。
        /// </summary>
        /// <param name="cssHref"></param>
        /// <param name="errorMessage">错误信息。</param>
        /// <returns></returns>
        public static IValidation HasCssLink(String cssHref, String errorMessage = null)
        {
            return new ValidationDelegateTaken("CSS 标签是否存在", ValidationType.Tag , (doc) =>
            {
                var htmlNode = doc.DocumentNode;
                var nodes = htmlNode.SelectNodes(@"//link[@rel='stylesheet']");
                if (nodes == null)
                {
                    return false;
                }
                return nodes.Where(n => cssHref == n.GetAttributeValue("href", null)).Count() > 0;
            }, errorMessage ?? string.Format("CSS 标签 \"{0}\" 不存在。", cssHref));
        }

        /// <summary>
        /// 验证 HTML Docuemnt 是否包含指定的 JS 文件。
        /// </summary>
        /// <param name="jsSrc"></param>
        /// <param name="errorMessage">错误信息。</param>
        /// <returns></returns>
        public static IValidation HasScriptLink(String jsSrc, String errorMessage = null)
        {
            return new ValidationDelegateTaken("JS 标签是否存在", ValidationType.Tag, (doc) =>
            {
                var htmlNode = doc.DocumentNode;
                var nodes = htmlNode.SelectNodes(@"//script[@src]");
                if (nodes == null)
                {
                    return false;
                }
                return nodes.Where(n => jsSrc == n.GetAttributeValue("src", null)).Count() > 0;
            }, errorMessage ?? string.Format("JS 标签 \"{0}\" 不存在。", jsSrc));
        }

        /// <summary>
        /// 验证 HTML Docuemnt 中引用的资源十是否存在。
        /// </summary>
        /// <param name="resourceBaseDirectory">资源的基础保存目录，将会基于此目录搜索相关资源文件。</param>
        /// <param name="errorMessage">错误信息。</param>
        /// <returns></returns>
        public static IValidation ResourcesExisting(String resourceBaseDirectory, String errorMessage = null)
        {
            return new ReferenceResourcesExistingValidation(resourceBaseDirectory);
        }

        public static IValidation XPathEquals(String templateFile, params String[] elementIds)
        {
            return new XPathValidation(templateFile, elementIds);
        }

        public static IValidation InternalALinkExisting(String searchBaseDirectory)
        {
            return new InternalALinkExistingValidation(searchBaseDirectory);
        }


    }
}
