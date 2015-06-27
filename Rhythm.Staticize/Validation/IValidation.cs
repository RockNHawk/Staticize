using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rhythm.Staticize
{
    /// <summary>
    /// 表示一个验证规则
    /// </summary>
    public interface IValidation
    {
        /// <summary>
        /// 执行验证。
        /// </summary>
        /// <param name="document">被验证的 HtmlDocument</param>
        /// <returns>获取验证不通过时的错误提示信息。</returns>
        string Validate(HtmlAgilityPack.HtmlDocument document, HtmlStaticizeContext status);

        /// <summary>
        /// 获取此验证的名称信息（用于向用户界面显示）。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 验证类型
        /// </summary>
        ValidationType Type { get; }
    }
}