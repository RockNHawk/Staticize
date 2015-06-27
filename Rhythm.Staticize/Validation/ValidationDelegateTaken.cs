using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rhythm.Staticize
{
    /// <summary>
    /// 定义一个对 HtmlDocument 进行验证的规则。
    /// </summary>
    public class ValidationDelegateTaken : IValidation
    {
        String errorMessage;
        System.Func<HtmlAgilityPack.HtmlDocument, bool> documentValidation;

        /// <summary>
        /// 初始化 HtmlDocumentValidation 的新实例。
        /// </summary>
        /// <param name="name">此验证的名称信息（用于向用户界面显示）。</param>
        /// <param name="documentValidation">一个委托，用于验证 HtmlDocument 是否符合规则。</param>
        /// <param name="errorMessage">验证不符合规则时的提示信息。</param>
        public ValidationDelegateTaken(String name, ValidationType validationType, System.Func<HtmlAgilityPack.HtmlDocument, bool> documentValidation, String errorMessage)
        {
            this.documentValidation = documentValidation;
            this.errorMessage = errorMessage;
            this.Name = name;
            this.Type = validationType;
            if (documentValidation == null)
            {
                throw new ArgumentNullException("documentValidation");
            }
            if (errorMessage == null)
            {
                throw new ArgumentNullException("errorMessage");
            }
        }

        /// <summary>
        /// 执行验证。
        /// </summary>
        /// <param name="document">被验证的 HtmlDocument</param>
        /// <returns>验证通过则返回true。</returns>
        public string Validate(HtmlAgilityPack.HtmlDocument document, HtmlStaticizeContext status)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }
            return documentValidation.Invoke(document) ? null : errorMessage;
        }

        /// <summary>
        /// 获取验证不通过时的错误提示信息。
        /// </summary>
        public String ErrorMessage { get { return errorMessage; } }

        /// <summary>
        /// 获取此验证的名称信息（用于向用户界面显示）。
        /// </summary>
        public String Name { get; private set; }

        public ValidationType Type { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
