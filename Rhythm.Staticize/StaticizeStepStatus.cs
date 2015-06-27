using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rhythm.Staticize
{
    /// <summary>
    /// 用于静态化状态跟踪。
    /// 支持异步跟踪静态化状态
    /// </summary>
    public class StaticizeStepStatus : IEnumerable<HtmlStaticizeContext>
    {
        internal int pageCount;

        /// <summary>
        /// 由于在内部生成页面是并行化的，因此此字段是 volatile 字段。
        /// </summary>
        volatile int volatileGeneratedPageCount;
        int generatedPageCount;
        internal int validatedPageCount;

        Dictionary<Uri, HtmlStaticizeContext> contexts;

        /// <summary>
        /// 初始化 StaticizeStepStatus 的新实例。
        /// </summary>
        public StaticizeStepStatus()
        {
            this.Errors = new List<Exception>();
            ValidationErrors = new List<ValidationResult>();
        }

        internal void AddGeneratedPageCount()
        {
            volatileGeneratedPageCount++;
            generatedPageCount = volatileGeneratedPageCount;
        }

        internal void AddValidatedPageCount()
        {
            validatedPageCount++;
        }

        //public StaticizeStepStatus(string id)
        //{
        //    if (id == null)
        //    {
        //        throw new ArgumentNullException("id");
        //    }
        //    this.Id = id;
        //}

        //public string Id { get; internal set; }
        internal void Init(HtmlStaticizeContext[] entries)
        {
            contexts = new Dictionary<Uri, HtmlStaticizeContext>(entries.Length);
            for (int j = 0; j < entries.Length; j++)
            {
                contexts.Add(entries[j].uri, entries[j]);
            }
            Urls = contexts.Keys;
            Items = entries;
        }

        /// <summary>
        /// 获取验证错误信息
        /// </summary>
        public IList<ValidationResult> ValidationErrors { get; private set; }

        public HtmlStaticizeContext this[Uri uri] { get { return contexts[uri]; } }

        public ICollection<Uri> Urls { get; private set; }

        /// <summary>
        /// 获取所有页面的静态化上下文
        /// </summary>
        public ICollection<HtmlStaticizeContext> Items { get; private set; }

        /// <summary>
        /// 获取静态化过程中发生的异常信息
        /// </summary>
        public IList<Exception> Errors { get; internal set; }

        public IEnumerator<HtmlStaticizeContext> GetEnumerator()
        {
            return contexts.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return contexts.Values.GetEnumerator();
        }

        StaticizeStep step;
        /// <summary>
        /// 获取当前静态化过程正处于哪个步骤
        /// </summary>
        public StaticizeStep Step
        {
            get { return step; }
            // 步骤变更后，会触发 StepChanged 事件
            internal set
            {
                var previus = step;
                step = value;
                if (previus != value)
                {
                    var @event = StepChanged;
                    if (@event != null)
                    {
                        @event(this, new StaticizeStepChangedEventArgs { Step = step, });
                    }
                }
            }
        }

        /// <summary>
        /// 阶段性状态变更事件。
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event StaticizeStepChangedEventHandler StepChanged;

        /// <summary>
        /// 获取当前的页面总数
        /// </summary>
        public int PageCount { get { return pageCount; } }

        /// <summary>
        /// 获取已生成的页面总数
        /// </summary>
        public int GeneratedPageCount { get { return generatedPageCount; } }

        /// <summary>
        /// 获取已验证的页面总数
        /// </summary>
        public int ValidatedPageCount { get { return validatedPageCount; } }
    }

}
