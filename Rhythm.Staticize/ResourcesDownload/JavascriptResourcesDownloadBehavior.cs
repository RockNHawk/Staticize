using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rhythm.Staticize
{
    /// <summary>
    /// 提供对被爬行页面中引用的 Javascript 资源的下载支持。
    /// </summary>
    public class JavascriptResourcesDownloadBehavior : ResourcesDownloadBaseBehavior
    {
        public JavascriptResourcesDownloadBehavior(String outputBaseDirectory, IUriResourcesFromLocalFileSystemReslover resourceFileReslover = null)
            : base(outputBaseDirectory, "//script[@src]", resourceFileReslover)
        {
        }

        protected override void OnResourceParsed(Uri[] resourceUris, HtmlStaticizeContext context)
        {
            base.OnResourceParsed(resourceUris, context);
            context.Resources.ReferenceJavascripts.AddRange(resourceUris);
        }


    }
}