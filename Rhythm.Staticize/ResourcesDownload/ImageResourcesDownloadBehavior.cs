using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rhythm.Staticize
{
    /// <summary>
    /// 提供对被爬行页面中引用的图片资源的下载支持。
    /// </summary>
    public class ImageResourcesDownloadBehavior : ResourcesDownloadBaseBehavior
    {
        /// <summary>
        /// 初始化 ImageResourcesDownloadBehavior 的新实例。
        /// </summary>
        /// <param name="outputBaseDirectory">图片输出文件夹</param>
        /// <param name="resourceFileReslover"></param>
        public ImageResourcesDownloadBehavior(String outputBaseDirectory, IUriResourcesFromLocalFileSystemReslover resourceFileReslover = null)
            : base(outputBaseDirectory, "//img[@src]", resourceFileReslover)
        {
        }

        /// <summary>
        /// 当资源URL被正确解析，即将被下载时回调
        /// </summary>
        /// <param name="resourceUris">当资源URL（集合）</param>
        /// <param name="context"></param>
        protected override void OnResourceParsed(Uri[] resourceUris, HtmlStaticizeContext context)
        {
            context.Resources.ReferenceImages.AddRange(resourceUris);
            base.OnResourceParsed(resourceUris, context);
        }

    }
}
