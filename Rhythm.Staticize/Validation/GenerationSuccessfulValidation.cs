using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rhythm.Staticize
{
    class GenerationSuccessfulValidation : IValidation
    {
        /// <summary>
        /// 获取 GenerationValidation 的实例。
        /// </summary>
        public static readonly IValidation Instance = new GenerationSuccessfulValidation();

        string IValidation.Validate(HtmlAgilityPack.HtmlDocument document, HtmlStaticizeContext status)
        {
            var errorMessage = new StringBuilder();
            if (status.GenerationError != null)
            {
                var err = status.GenerationError;
                errorMessage.AppendFormat("生成HTML期间发生错误：{0}\r\n{1}\r\n", err.Message, err.ToString());
            }
            if (status.DocumentLoadError != null)
            {
                var err = status.DocumentLoadError;
                errorMessage.AppendFormat("加载HTML文档树期间发生错误：{0}\r\n{1}\r\n", err.Message, err.ToString());
            }
            return errorMessage.Length == 0 ? null : errorMessage.ToString();
        }

        string IValidation.Name { get { return "页面HTML是否成功生成。"; } }


        public ValidationType Type { get { return ValidationType.Tag; } }


    }
}
