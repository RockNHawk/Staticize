using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rhythm.Staticize
{
    /// <summary>
    /// 提供对被爬行页面中引用资源的下载支持。
    /// </summary>
    public class ResourcesDownloadBaseBehavior : IBehavior
    {
        String resourcesNodeSelectPath;
        String outputDirectory;
        IUriResourcesFromLocalFileSystemReslover fileReslover;

        /// <summary>
        /// 初始化 InterceptorForResourcesDownloadBase 的新实例。
        /// </summary>
        /// <param name="outputBaseDirectory">引用资源的输出文件夹。</param>
        /// <param name="resourcesHtmlNodeSelectPath">引用资源的HTML标签XPath表达式。</param>
        /// <param name="resourceFileReslover">用于将Uri路径转换为本地路径。</param>
        public ResourcesDownloadBaseBehavior(String outputBaseDirectory, String resourcesHtmlNodeSelectPath, IUriResourcesFromLocalFileSystemReslover resourceFileReslover = null)
        {
            this.outputDirectory = outputBaseDirectory;
            this.resourcesNodeSelectPath = resourcesHtmlNodeSelectPath;
            this.fileReslover = resourceFileReslover ?? new DefaultUriToLocalFilePathReslover();
            if (outputBaseDirectory == null)
            {
                throw new ArgumentNullException("outputBaseDirectory");
            }
            if (resourcesHtmlNodeSelectPath == null)
            {
                throw new ArgumentNullException("resourcesHtmlNodeSelectPath");
            }
        }

        Dictionary<String, Object> files = new Dictionary<string, object>(100);
        Dictionary<String, Object> directories = new Dictionary<string, object>(20);
        public void  Process(HtmlAgilityPack.HtmlDocument document, HtmlStaticizeContext context)
        {
            Uri documentUri = context.Uri;
            String baseUrl = documentUri.GetParent();// GetParent(documentUri);

            var htmlNode = document.DocumentNode;
            var nodes = htmlNode.SelectNodes(resourcesNodeSelectPath);
            if (nodes == null || nodes.Count == 0)
            {
                return;
            }
            var srcAttributes = GetSrcAttributes(nodes);
            if (srcAttributes == null || srcAttributes.Count() == 0)
            {
                return;
            }
            var parsedSrcUris = ParseResourcesUris(documentUri, baseUrl, srcAttributes);
            if (parsedSrcUris == null || parsedSrcUris.Length == 0)
            {
                return;
            }

            OnResourceParsed(parsedSrcUris, context);

            for (int i = 0; i < parsedSrcUris.Length; i++)
            {
                var uri = parsedSrcUris[i];
                String localPath = fileReslover.ResloveLocalPath(uri);
                if (String.IsNullOrEmpty(localPath))
                {
                    continue;
                }
                var localDirectory = System.IO.Path.Combine(outputDirectory, System.IO.Path.GetDirectoryName(localPath));
                if (!directories.ContainsKey(localDirectory) && !System.IO.Directory.Exists(localDirectory))
                {
                    System.IO.Directory.CreateDirectory(localDirectory);
                    //并发 patch
                    try
                    {
                        directories.Add(localDirectory, null);
                    }
                    catch (Exception)
                    {
                    }
                }

                String saveFile = System.IO.Path.Combine(outputDirectory, localPath);
                //并发 patch
                try
                {
                    //已存在相同文件，则跳过。为避免并发写同一个文件。
                    if (files.ContainsKey(saveFile))
                    {
                        continue;
                    }
                    files.Add(saveFile, null);
                }
                catch (Exception)
                {
                }
                if (System.IO.File.Exists(saveFile))
                {
                    continue;
                }
                if (this.fileReslover.TryCopyFromLocal(uri, saveFile))
                {
                    continue;
                }
                using (System.Net.WebClient wc = new System.Net.WebClient())
                {
                    try
                    {
                        wc.DownloadFile(uri, saveFile);
                    }
                    catch (Exception ex)
                    {
                        //修复WebClient 文件不存在仍然本地保存了一个空文件
                        System.IO.File.Delete(saveFile);
                        context.Errors.Add(new ResourcesDownloadException(String.Format(@"下载资源 ""{0}"" 时发生异常。", uri.ToString()), ex)
                        {
                            Url = uri,
                        });
                        continue;
                    }
                }
            }
        }

        ///// <summary>
        ///// 获取页面的Uri所在的目录名称。（仅支持页面）
        ///// </summary>
        ///// <param name="uri"></param>
        ///// <returns>上一级的URI Address。</returns>
        //static string GetParent(Uri uri)
        //{
        //    String documentUriString = uri.ToString();
        //    int lastSlash = documentUriString.LastIndexOf('/');
        //    String documenBaseDir = documentUriString.Substring(0, lastSlash);
        //    if (lastSlash == -1)
        //    {
        //        documenBaseDir = documentUriString;
        //    }
        //    else
        //    {
        //        //特殊处理
        //        //如果最后还是“/”（有的URL不标准，路径中有两个“//”，如http://localhost:90/Admin/Blogs///17.html）
        //        while (
        //            documenBaseDir[documenBaseDir.Length - 1] == '/' ||
        //            documenBaseDir[documenBaseDir.Length - 1] == '\\'
        //            )
        //        {
        //            documenBaseDir = documenBaseDir.Substring(0, documenBaseDir.Length - 1);
        //        }
        //    }
        //    return documenBaseDir;
        //}

        /// <summary>
        /// 获取资源 Html Node 的“src”属性。
        /// </summary>
        /// <param name="nodes">Html Node集合</param>
        /// <returns>排除空了值的“src”属性集合。</returns>
        protected virtual string[] GetSrcAttributes(HtmlAgilityPack.HtmlNodeCollection nodes)
        {
            var srcAttributes = (from node in nodes
                                 where !String.IsNullOrWhiteSpace(node.GetAttributeValue("src", null))
                                 select node.GetAttributeValue("src", null)).ToArray();
            return srcAttributes;
        }

        /// <summary>
        /// 从“src”属性的值创建Uri对象。
        /// </summary>
        /// <param name="srcAttributes">“src”属性集合。</param>
        /// <returns>Uri对象集合。</returns>
        Uri[] ParseResourcesUris(Uri documentUri, string documenBaseDir, string[] srcAttributes)
        {
            Uri parseUri = null;
            var parsedImgSrcUris = (from src in srcAttributes
                                    select Uri.TryCreate(src, UriKind.RelativeOrAbsolute, out parseUri) ? parseUri : null).Where(m => m != null).ToArray();

            var filter = (from uri in parsedImgSrcUris
                          where uri.IsAbsoluteUri == false || (uri.IsAbsoluteUri && uri.Host == documentUri.Host)
                          select uri.IsAbsoluteUri ? uri : CreateAbsoluteUri(documentUri, documenBaseDir, uri)).Distinct().ToArray();
            return filter;
        }

        /// <summary>
        /// 当资源URL被正确解析，即将被下载时回调
        /// </summary>
        /// <param name="resourceUris">当资源URL（集合）</param>
        /// <param name="context"></param>
        protected virtual void OnResourceParsed(Uri[] resourceUris, HtmlStaticizeContext context)
        {

        }


        Uri CreateAbsoluteUri(Uri documentUri, string documenBaseDir, Uri uri)
        {
            String str = uri.ToString();
            if (str[0] == '/')
            {
                String format = String.Format("{0}://{1}:{2}{3}", documentUri.Scheme, documentUri.Host, documentUri.Port, uri.ToString());
                return new Uri(format);
            }
            else
            {

                String format = String.Format("{0}/{1}", documenBaseDir, uri.ToString());
                return new Uri(format);
            }
        }


    }


}
