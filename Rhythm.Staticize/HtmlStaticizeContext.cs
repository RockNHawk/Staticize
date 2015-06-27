using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rhythm.Staticize
{
    /// <summary>
    /// <para>网页静态化时的上下文信息。</para>
    /// <para>提供对爬行网页过程中状态信息存储的支持。</para>
    /// <para>提供错误信息列表。</para>
    /// <para></para>
    /// </summary>
    public class HtmlStaticizeContext
    {
        internal Uri uri;
        internal String fileName;
        internal Exception generationError;
        internal IList<ValidationResult> validationResults;

        /// <summary>
        /// 初始化 HtmlStaticizeContext 的新实例
        /// </summary>
        public HtmlStaticizeContext()
        {
            Resources = new ResourcesManager();
            Errors = new List<Exception>();
        }

        /// <summary>
        /// 获取网页的Uri信息。
        /// </summary>
        public Uri Uri { get { return uri; } }

        /// <summary>
        /// 获取是否生成失败。
        /// 如果不为null，表示 HTML 生成失败。
        /// </summary>
        public System.Exception GenerationError { get { return generationError; } }

        /// <summary>
        /// 获取是否加载 HTML 失败（与 XMLDocument.LoadXML 方法同理，如果 Load 失败，表示 HTML document 格式不正确）。
        /// </summary>
        public System.Exception DocumentLoadError { get; internal set; }

        /// <summary>
        /// 此页面静态化过程中的错误信息。
        /// 如发生404等错误均会在此记录。
        /// </summary>
        public IList<Exception> Errors { get; internal set; }

        /// <summary>
        /// 网页引用资源信息
        /// </summary>
        public ResourcesManager Resources { get; internal set; }

        /// <summary>
        /// 对此网页静态化后的验证结果（集合）
        /// </summary>
        public IList<ValidationResult> ValidationResults { get { return validationResults; } }
    }

}
