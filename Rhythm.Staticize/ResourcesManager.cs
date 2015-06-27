using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rhythm.Staticize
{

    /// <summary>
    /// 提供对被静态化网页总引用资源的计数支持。
    /// </summary>
    public class ResourcesManager
    {
        /// <summary>
        /// 初始化 ResourcesManager 的新实例
        /// </summary>
        public ResourcesManager()
        {
            ReferenceCsses = new List<Uri>(3);
            ReferenceJavascripts = new List<Uri>(3);
            ReferenceImages = new List<Uri>(5);
            NotExistsFiles = new Dictionary<Uri, String>();
        }

        /// <summary>
        /// 获取页面 HTML 中引用的 CSS
        /// </summary>
        public IList<Uri> ReferenceCsses { get; private set; }

        /// <summary>
        /// 获取页面 HTML 中引用的 JS
        /// </summary>
        public IList<Uri> ReferenceJavascripts { get; private set; }

        /// <summary>
        /// 获取页面 HTML 中引用的图片
        /// </summary>
        public IList<Uri> ReferenceImages { get; private set; }

        /// <summary>
        /// 获取页面 HTML 中有引用但实际不存在的图片。
        /// </summary>
        public IDictionary<Uri, String> NotExistsFiles { get; private set; }
    }

}