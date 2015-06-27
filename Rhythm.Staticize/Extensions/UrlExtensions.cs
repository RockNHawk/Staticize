using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rhythm.Staticize
{
    public static class UrlExtensions
    {
        /// <summary>
        /// 获取页面的Uri所在的目录名称。（仅支持页面）
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>所在的目录名称</returns>
        public static string GetFileDirectory(this Uri uri)
        {
            //input:http://xxx/xx.html
            //return:/
            var baseUrlString = uri.GetParent();
            var baseUrl = new Uri(baseUrlString);
            return baseUrl.LocalPath;
        }

        /// <summary>
        /// 获取URI的上一级的URI Address。
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>上一级的URI Address。</returns>
        public static string GetParent(this Uri uri)
        {
            //input:http://xxx/xx.html
            //return:http://xxx
            String uriString = uri.ToString();
            int lastSlash = uriString.LastIndexOf('/');
            if (lastSlash == -1)
            {
                return uriString;
            }
            else
            {
                String baseDir = uriString.Substring(0, lastSlash + 1);
                return baseDir;//.TidyUri();
            }
        }
         
    }
}
