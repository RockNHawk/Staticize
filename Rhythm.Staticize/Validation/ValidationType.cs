using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rhythm.Staticize
{
    /// <summary>
    /// 验证的类型（标签完整性、链接完整性、资源文件完整性）。
    /// </summary>
    public enum ValidationType
    {
        [System.ComponentModel.DataAnnotations.Display(Name = "其它")]
        Other,
        /// <summary>
        /// 标签完整性
        /// </summary>
        [System.ComponentModel.DataAnnotations.Display(Name = "标签完整性")]
        Tag,
        /// <summary>
        /// 链接完整性
        /// </summary>
        [System.ComponentModel.DataAnnotations.Display(Name = "链接完整性")]
        Link,
        /// <summary>
        /// 资源文件完整性
        /// </summary>
        [System.ComponentModel.DataAnnotations.Display(Name = "资源文件完整性")]
        Resource,
        /// <summary>
        /// 内容正确性
        /// </summary>
        [System.ComponentModel.DataAnnotations.Display(Name = "内容正确性")]
        Content,
    }
}
