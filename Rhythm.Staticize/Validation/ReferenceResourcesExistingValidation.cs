using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rhythm.Staticize
{
    /// <summary>
    /// 检查HTML中引用的资源文件是否在文件系统中存在。
    /// <para>它会搜索网页中引用的所有CSS、JS、图片文件，然后在本地静态化目录查找是否存在这些文件。</para>
    /// </summary>
    public class ReferenceResourcesExistingValidation : IValidation
    {
        Dictionary<String, Boolean> exisitingFiles = new Dictionary<string, bool>(1000);
        String outputDir;
        IUriResourcesFromLocalFileSystemReslover fileReslover;

        /// <summary>
        /// 初始化 ReferenceResourcesExistsValidation 的新实例。
        /// </summary>
        /// <param name="resourceBaseDir">资源的基础保存目录，将会基于此目录搜索相关资源文件。</param>
        /// <param name="resourceFileReslover">定义将Uri路径转换为本地文件系统路径。</param>
        public ReferenceResourcesExistingValidation(String resourceBaseDir, IUriResourcesFromLocalFileSystemReslover resourceFileReslover = null)
        {
            this.outputDir = resourceBaseDir;
            this.fileReslover = resourceFileReslover ?? DefaultUriToLocalFilePathReslover.Instance;
        }

        public string Validate(HtmlAgilityPack.HtmlDocument document, HtmlStaticizeContext status)
        {
            var resources = status.Resources;
            String resultCss = Validate(resources.ReferenceCsses, status);
            String resultJs = Validate(resources.ReferenceJavascripts, status);
            String resultImage = Validate(resources.ReferenceImages, status);
            return (
                String.IsNullOrEmpty(resultCss) &&
                String.IsNullOrEmpty(resultImage) &&
                String.IsNullOrEmpty(resultJs)
                ) ? null : (
                String.Format("{0}\r\n{1}\r\n{2}", resultCss, resultJs, resultImage)
                );
        }

        string Validate(IList<Uri> list, HtmlStaticizeContext status)
        {
            if (list == null)
            {
                return null;
            }
            Boolean isValid = true;
            var errorMessage = new StringBuilder();
            foreach (var uri in list)
            {
                String fileName = fileReslover.ResloveLocalPath(uri);
                if (String.IsNullOrEmpty(fileName))
                {
                    continue;
                }
                String physicalFilePath = System.IO.Path.Combine(outputDir, fileName);
                bool fileExists = false;
                bool hasKey = false;
                try
                {
                    //并发修改 patch
                    hasKey = exisitingFiles.TryGetValue(physicalFilePath, out fileExists);
                }
                catch (Exception)
                {
                }
                if (!hasKey || !fileExists)
                {
                    if (!hasKey)
                    {
                        fileExists = System.IO.File.Exists(physicalFilePath);
                    }
                    if (!fileExists)
                    {
                        isValid = false;
                        status.Resources.NotExistsFiles.Add(uri, physicalFilePath);
                        errorMessage.AppendFormat("资源 \"{0}\" 未能在本地预期的路径 \"{1}\" 中找到。\r\n", uri.ToString(), physicalFilePath);
                        {
                            var ex = status.GenerationError;
                            if (ex != null)
                            {
                                errorMessage.AppendLine("这可能是由于请求文件时发生异常造成的，以下是异常信息:");
                                errorMessage.AppendFormat("{0}:\r\n{1}\r\n\r\n", ex.Message, ex.ToString());
                            }
                        }
                    }
                    try
                    {
                        //并发修改 patch
                        exisitingFiles.Add(physicalFilePath, fileExists);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return isValid ? null : errorMessage.ToString();
        }

        //StringBuilder errorMessage;// = new StringBuilder();
        //string IValidation.ErrorMessage
        //{
        //    get { return errorMessage == null ? null : errorMessage.ToString(); }
        //}

        public string Name { get { return "网页引用的资源文件是否存在"; } }

        public ValidationType Type { get { return ValidationType.Resource; } }


    }
}
