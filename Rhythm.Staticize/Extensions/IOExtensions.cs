using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rhythm.Staticize
{
    public static class IOExtensions
    {
        public static bool IsDirectorySeparator(this char chr)
        {
            return chr == '/' || chr == '\\';
        }

        public static int IndexOfDirectorySeparator(this string path)
        {
            if (path == null)
            {
                return -1;
            }
            int index1 = path.IndexOf('/');
            int index2 = path.IndexOf('\\');
            return index1 > index2 ? index1 : index2;
        }


        /// <summary>
        /// 移光最前面的斜杠，不管是正斜还是反斜。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string RemoveStartDirectorySeparator(this string path)
        {
            if (path == null)
            {
                return null;
            }
            if (path.Length == 0)
            {
                return path;
            }
            if (!path[0].IsDirectorySeparator())
            {
                return path;
            }
            do
            {
                path = path.Substring(1);
            } while (path.Length != 0 && path[0].IsDirectorySeparator());
            return path;
        }

        /// <summary>
        /// 移光最后的斜杠，不管是正斜还是反斜。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string RemoveLastDirectorySeparator(this string path)
        {
            if (path == null)
            {
                return null;
            }
            int length = path.Length;
            if (length == 0)
            {
                return path;
            }
            if (!path[(length - 1)].IsDirectorySeparator())
            {
                return path;
            }
            do
            {
                // 移掉最后一个
                path = path.Substring(0, length - 1);
                length--;
            } while (length > 0 && path[length - 1].IsDirectorySeparator());
            return path;
        }




    }
}
