using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rhythm.Staticize
{
    /// <summary>
    /// 提供对被爬行页面中引用的 CSS 资源的下载支持。
    /// </summary>
    public class CssResourcesDownloadBehavior : ResourcesDownloadBaseBehavior
    {
        public CssResourcesDownloadBehavior(String outputBaseDirectory, IUriResourcesFromLocalFileSystemReslover resourceFileReslover = null)
            : base(outputBaseDirectory, @"//link[@rel='stylesheet']", resourceFileReslover)
        {
        }

        protected override string[] GetSrcAttributes(HtmlAgilityPack.HtmlNodeCollection nodes)
        {
            var srcAttributes = (from node in nodes
                                 where !String.IsNullOrWhiteSpace(node.GetAttributeValue("href", null))
                                 select node.GetAttributeValue("href", null)).ToArray();
            return srcAttributes;
        }

        protected override void OnResourceParsed(Uri[] resourceUris, HtmlStaticizeContext context)
        {
            base.OnResourceParsed(resourceUris, context);
            context.Resources.ReferenceCsses.AddRange(resourceUris);
        }

    }
}
