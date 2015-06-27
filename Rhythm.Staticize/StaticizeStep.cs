using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rhythm.Staticize
{
    /// <summary>
    /// 表示静态化过程关键步骤。
    /// </summary>
    public enum StaticizeStep
    {
        /// <summary>
        /// 正在初始化
        /// </summary>
        [System.ComponentModel.DataAnnotations.Display(Name = "正在初始化")]
        Initialize,
        /// <summary>
        /// 正在生成 HTML
        /// </summary>
        [System.ComponentModel.DataAnnotations.Display(Name = "正在生成 HTML")]
        GenerationHtml,
        /// <summary>
        /// HTML 生成完成
        /// </summary>
        [System.ComponentModel.DataAnnotations.Display(Name = "HTML 生成完成")]
        GenerationHtmlCompleted,
        /// <summary>
        /// 正在验证
        /// </summary>
        [System.ComponentModel.DataAnnotations.Display(Name = "正在验证")]
        Validation,
        /// <summary>
        /// 验证完成
        /// </summary>
        [System.ComponentModel.DataAnnotations.Display(Name = "验证完成")]
        ValidationCompleted,
        /// <summary>
        /// 已完成
        /// </summary>
        [System.ComponentModel.DataAnnotations.Display(Name = "已完成")]
        Completed,
        /// <summary>
        /// 静态化过程被意外终止
        /// <para>静态化执行过程中意外停止了，可能是线程 Crash 或计算机关机造成的。</para>
        /// </summary>
        [System.ComponentModel.DataAnnotations.Display(Name = "静态化过程被意外终止", Description = "静态化执行过程中意外停止了，可能是线程 Crash 或计算机关机造成的。")]
        Crashed,
    }

}
