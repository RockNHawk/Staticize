using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rhythm.Staticize
{
    /// <summary>
    /// 添加 IBehavior ，它会在 HTML 被生成，并加载后执行。
    /// 因此你可以使用 IBehavior 对 HTML 进行读取，它会在验证之前执行。
    /// </summary>
    public interface IBehavior
    {
        void Process(HtmlAgilityPack.HtmlDocument document, HtmlStaticizeContext context);
    }
}
