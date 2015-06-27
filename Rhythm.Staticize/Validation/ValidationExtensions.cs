using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rhythm.Staticize
{
    public static class ValidationExtensions
    {
        /// <summary>
        /// 执行所有 HTML 检查
        /// </summary>
        /// <param name="context"></param>
        /// <returns>HTML 检查结果。</returns>
        public static IList<ValidationResult> Validate(this IEnumerable<IValidation> validations, HtmlAgilityPack.HtmlDocument document, HtmlStaticizeContext context)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (validations == null)
            {
                return null;
            }
            var validationResult = new List<ValidationResult>();
            foreach (var vd in validations)
            {
                var errorMessage = vd.Validate(document, context);
                if (errorMessage != null && errorMessage.Length > 0)
                {
                    validationResult.Add(new ValidationResult
                    {
                        Uri = context.Uri,
                        ValidationType = vd.Type,
                        Name = vd.Name,
                        Message = errorMessage,
                    });
                }
            }
            return validationResult;
        }

        public static IEnumerable<ValidationResult> GetValidationResults(this IEnumerable<HtmlStaticizeContext> staticizeContext)
        {
            List<ValidationResult> all = new List<ValidationResult>();
            foreach (var item in staticizeContext)
            {
                if (item.ValidationResults != null && item.ValidationResults.Count() > 0)
                {
                    all.AddRange(item.ValidationResults);
                }
            }
            return all;
        }

        public static void Save(this IEnumerable<ValidationResult> validateResults, String filePath)
        {
            if (validateResults == null)
            {
                return;
            }
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            foreach (var item in validateResults)
            {
                var message = item.Message;
                if (message == null || message.Length == 0)
                {
                    continue;
                }
                builder.AppendFormat("=======================\r\n");
                builder.AppendFormat("对页面 {0} 的验证结果：\r\n\r\n", item.Uri);
                builder.AppendFormat("验证 [{0}] 不通过:\r\n{1}\r\n", item.Name, message);
            }
            if (builder.Length == 0)
            {
                return;
            }
            System.IO.File.AppendAllText(filePath, builder.ToString());
        }

    }
}
