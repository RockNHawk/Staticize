using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rhythm.Staticize
{
    public class InternalALinkExistingValidation : IValidation
    {
        String searchDirectory;
        IUriResourcesFromLocalFileSystemReslover fileReslover;

        Dictionary<String, bool> files = new Dictionary<string, bool>(5000);

        /// <summary>
        /// 初始化 InternalALinkValidation 的新实例。
        /// </summary>
        /// <param name="searchBaseDirectory">引用资源的输出文件夹。</param>
        /// <param name="resourceFileReslover">用于将Uri路径转换为本地路径。</param>
        public InternalALinkExistingValidation(String searchBaseDirectory, IUriResourcesFromLocalFileSystemReslover resourceFileReslover = null)
        {
            this.searchDirectory = searchBaseDirectory;
            this.fileReslover = resourceFileReslover ?? DefaultUriToLocalFilePathReslover.Instance;
            if (searchBaseDirectory == null)
            {
                throw new ArgumentNullException("outputBaseDirectory");
            }
        }

        public string Validate(HtmlAgilityPack.HtmlDocument document, HtmlStaticizeContext status)
        {
            //得到当前页面的目录
            string documentDir = status.Uri.GetFileDirectory();
            var errorMessage = new StringBuilder();
            var htmlNode = document.DocumentNode;
            var nodes = htmlNode.SelectNodes("//a");
            if (nodes == null || nodes.Count == 0)
            {
                return null;
            }
            foreach (var aNode in nodes)
            {
                string href = aNode.GetAttributeValue("href", null);
                if (string.IsNullOrWhiteSpace(href) || href[0] == '#')
                {
                    continue;
                }
                //如果href是相对当前页面来说的：<a href="1.html" />
                if (!href[0].IsDirectorySeparator())
                {
                    href = documentDir + href;
                }

                Uri uri;
                if (!Uri.TryCreate(href, UriKind.RelativeOrAbsolute, out uri))
                {
                    continue;
                }
                //这里可以增加对站内域名的判断
                if (uri.IsAbsoluteUri && !string.IsNullOrEmpty(uri.Host))
                {
                    continue;
                }
                string local = fileReslover.ResloveLocalPath(uri);
                if (string.IsNullOrEmpty(local))
                {
                    continue;
                }
                string localPath = System.IO.Path.Combine(searchDirectory, local);
                bool isExisting;
                if (!files.TryGetValue(localPath, out isExisting))
                {
                    isExisting = System.IO.File.Exists(localPath);
                    try
                    {
                        files.Add(localPath, isExisting);
                    }
                    catch (Exception)
                    {
                    }
                }
                if (!isExisting)
                {
                    errorMessage.AppendFormat("本地不存在链接 \"{0}\" 所指向的文件 \"{1}\"。", uri.ToString(), localPath);
                }
            }
            return errorMessage.Length == 0 ? null : errorMessage.ToString();
        }

        public string Name { get { return "检查页面HTML中的站内A链接指向的页面是在本地存在对应文件。"; } }

        public ValidationType Type { get { return ValidationType.Link; } }


        public override string ToString() { return ((IValidation)this).Name; }

    }
}
