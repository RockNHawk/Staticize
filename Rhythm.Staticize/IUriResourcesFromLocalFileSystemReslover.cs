using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rhythm.Staticize
{
    /// <summary>
    /// 定义将Uri路径转换为本地文件系统文件的支持。
    /// </summary>
    public interface IUriResourcesFromLocalFileSystemReslover
    {
        /// <summary>
        /// 定义将Uri转换为本地路径。
        /// </summary>
        /// <param name="uri">表示一个资源的链接。</param>
        /// <returns>该资源的本地路径。</returns>
        String ResloveLocalPath(Uri uri);

        /// <summary>
        /// 如果被静态化的网站和静态化程序处于同一个计算机中，尝试直接从本地复制文件。
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="saveFilePath"></param>
        /// <returns>是否复制成功</returns>
        Boolean TryCopyFromLocal(Uri uri, String saveFilePath);
    }

    /// <summary>
    /// 默认 IUriToLocalFilePathReslover 的实现。
    /// </summary>
    public class DefaultUriToLocalFilePathReslover : IUriResourcesFromLocalFileSystemReslover
    {
        /// <summary>
        /// 获取 DefaultUriToLocalFilePathReslover 的实例。
        /// </summary>
        public static readonly IUriResourcesFromLocalFileSystemReslover Instance = new DefaultUriToLocalFilePathReslover();

        String baseDir = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// 定义将Uri转换为本地路径。
        /// </summary>
        /// <param name="uri">表示一个资源的链接。</param>
        /// <returns>该资源的本地路径。</returns>
        public string ResloveLocalPath(Uri uri)
        {
            return FormatUriToLocalPath(uri);
        }

        static string FormatUriToLocalPath(Uri uri)
        {
            String localPath = uri.IsAbsoluteUri ? uri.LocalPath : uri.ToString();
            localPath = localPath[0] == '/' || localPath[0] == '\\' ? localPath.Substring(1, localPath.Length - 1) : localPath;
            return localPath;
        }

        /// <summary>
        /// 如果被静态化的网站和静态化程序处于同一个计算机中，尝试直接从本地复制文件。
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="saveFilePath"></param>
        /// <returns>是否复制成功</returns>
        public bool TryCopyFromLocal(Uri uri, string saveFilePath)
        {
            String resourceFilePath = System.IO.Path.Combine(baseDir, FormatUriToLocalPath(uri));
            if (System.IO.File.Exists(resourceFilePath))
            {
                try
                {
                    System.IO.File.Copy(resourceFilePath, saveFilePath);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }

    }

}