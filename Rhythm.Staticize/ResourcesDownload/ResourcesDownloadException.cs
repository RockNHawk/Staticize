using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rhythm.Staticize
{
    [Serializable]
    public class ResourcesDownloadException : System.Exception
    {

        /// <summary>
        /// 初始化此类的新实例
        /// </summary>
        public ResourcesDownloadException()
        { }

        /// <summary>
        /// 使用指定的错误信息初始化此类的新实例。
        /// </summary>
        /// <param name="message">解释异常原因的错误信息</param>
        public ResourcesDownloadException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// 使用指定错误消息和对作为此异常原因的内部异常的引用来初始化此类的新实例。
        /// </summary>
        /// <param name="message">解释异常原因的错误信息</param>
        /// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个 null 引用。</param>
        public ResourcesDownloadException(string message, System.Exception innerException)
            : base(message, innerException)
        {

        }

        public Uri Url { get; set; }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            base.GetObjectData(info, context);
            info.AddValue("Url", this.Url, typeof(string));
        }
    }
}
