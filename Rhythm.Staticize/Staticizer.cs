using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
//using HtmlAgilityPack;

namespace Rhythm.Staticize
{
    /// <summary>
    /// 静态化关键状态（StaticizeStep）变化的事件委托。
    /// </summary>
    /// <param name="sender">表示当前状态的对象</param>
    /// <param name="e">关键状态变更事件参数</param>
    public delegate void StaticizeStepChangedEventHandler(StaticizeStepStatus sender, StaticizeStepChangedEventArgs e);

    /// <summary>
    /// 静态化类库的入口。
    /// </summary>
    public class Staticizer
    {
        /// <summary>
        /// 执行静态化
        /// </summary>
        /// <param name="pages">要静态化的页面列表。Key为页面绝对URL，Value为这个页面保存在本地的路径。URL和Value必须是唯一的。</param>
        /// <param name="stepTaken">静态化状态，默认请传入此实例，它提供对异步线程获取静态化状态的支持。</param>
        /// <returns>静态化状态，与传入的 stepTaken 引用一致。</returns>
        public StaticizeStepStatus Staticize(IEnumerable<KeyValuePair<Uri, String>> pages, StaticizeStepStatus stepTaken)
        {
            if (pages == null)
            {
                throw new ArgumentNullException("pages");
            }
            if (stepTaken == null)
            {
                throw new ArgumentNullException("stepTaken");
            }
            #region 初始化
            stepTaken.Step = StaticizeStep.Initialize;

            int pageCount = pages.Count();
            stepTaken.pageCount = pageCount;
            // 创建 Context 对象，每个页面一个 Context 
            HtmlStaticizeContext[] entries = new HtmlStaticizeContext[pageCount];
            {
                int i = 0;
                foreach (var address in pages)
                {
                    entries[i] = new HtmlStaticizeContext
                    {
                        uri = address.Key,
                        fileName = address.Value,
                    };
                    i++;
                }
            }
            stepTaken.Init(entries);

            AddValidation(GenerationSuccessfulValidation.Instance);

            #endregion


            stepTaken.Step = StaticizeStep.GenerationHtml;

            // 生成 HTML
            Generate(entries, stepTaken);
            stepTaken.Step = StaticizeStep.GenerationHtmlCompleted;

            #region 验证

            stepTaken.Step = StaticizeStep.Validation;
            if (
                (m_Behaviors != null && m_Behaviors.Count > 0)
                || (m_Validations != null && m_Validations.Count > 0)
                )
            {
                for (int j = 0; j < entries.Length; j++)
                {
                    var entry = entries[j];
                    // 如果 generationError 不为null，表示 HTML 生成失败。
                    if (entry.generationError != null)
                    {
                        var ex = entry.generationError;
                        var vd = new ValidationResult()
                        {
                            ValidationType = ValidationType.Tag,
                            Uri = entry.uri,
                            Name = "页面HTML是否成功生成。",
                            Message = string.Format("生成HTML期间发生错误：{0}\r\n{1}\r\n", ex.Message, ex.ToString()),
                            Exception = ex,
                        };
                        entry.validationResults = new ValidationResult[] { vd };
                        stepTaken.ValidationErrors.Add(vd);
                        stepTaken.validatedPageCount++;
                        continue;
                    }
                    // load document dom
                    var doc = new HtmlAgilityPack.HtmlDocument();
                    // 尝试加载 document
                    try
                    {
                        doc.Load(entry.fileName, System.Text.Encoding.UTF8);
                    }
                    catch (Exception ex)
                    {
                        // 加载 document失败
                        entry.DocumentLoadError = ex;
                        var vd = new ValidationResult()
                        {
                            ValidationType = ValidationType.Tag,
                            Uri = entry.uri,
                            Name = "页面HTML是否成功生成。",
                            Message = string.Format("加载HTML文档树期间发生错误：{0}\r\n{1}\r\n", ex.Message, ex.ToString()),
                            Exception = ex,
                        };
                        entry.validationResults = new ValidationResult[] { vd };
                        stepTaken.ValidationErrors.Add(vd);
                        stepTaken.AddValidatedPageCount();
                        continue;
                    }

                    if (m_Behaviors != null && m_Behaviors.Count > 0)
                    {
                        for (int k = 0; k < m_Behaviors.Count; k++)
                        {
                            m_Behaviors[k].Process(doc, entry);
                        }
                    }
                    if (m_Validations != null && m_Validations.Count > 0)
                    {
                        Validate(doc, entry, stepTaken);
                    }
                    stepTaken.AddValidatedPageCount();
                }
            }
            stepTaken.Step = StaticizeStep.ValidationCompleted;

            #endregion

            // add context errors results to status
            {
                var all = stepTaken.Errors;
                for (int i = 0; i < entries.Length; i++)
                {
                    var items = entries[i].Errors;
                    if (items != null && items.Count > 0)
                    {
                        all.AddRange(items);
                    }
                }
            }

            stepTaken.Step = StaticizeStep.Completed;
            return stepTaken;
        }

        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="context"></param>
        /// <param name="stepTaken"></param>
        void Validate(HtmlAgilityPack.HtmlDocument doc, HtmlStaticizeContext context, StaticizeStepStatus stepTaken)
        {
            if (this.m_Validations != null)
            {
                var result = m_Validations.Validate(doc, context);
                if (result != null && result.Count > 0)
                {
                    if (context.validationResults == null)
                    {
                        context.validationResults = result;
                    }
                    else
                    {
                        context.validationResults.AddRange(result);
                    }
                    stepTaken.ValidationErrors.AddRange(result);
                }
            }
        }

        /// <summary>
        /// 生成
        /// </summary>
        /// <param name="entries"></param>
        /// <param name="step"></param>
        void Generate(HtmlStaticizeContext[] entries, StaticizeStepStatus step)
        {
            System.Threading.Tasks.Parallel.ForEach(entries, (entry) =>
            {
                using (var wc = new WebClient())
                {
                    try
                    {
                        wc.DownloadFile(entry.uri, entry.fileName);
                        step.AddGeneratedPageCount();
                    }
                    catch (Exception ex)
                    {
                        entry.generationError = ex;
                        entry.Errors.Add(ex);
                        //step.Errors.Add(ex);
                    }
                }
            });
            //// batch download html file
            //using (var wc = new WebClient())
            //{
            //    for (int j = 0; j < entries.Length; j++)
            //    {
            //        var entry = entries[j];
            //        // may be some url down failure
            //        // should log error
            //        try
            //        {
            //            wc.DownloadFile(entry.uri, entry.fileName);
            //        }
            //        catch (Exception ex)
            //        {
            //            entry.generationError = ex;
            //            entry.Errors.Add(ex);
            //        }
            //    }
            //}
        }


        List<IBehavior> m_Behaviors;
        /// <summary>
        /// 添加 IBehavior ，它会在 HTML 被生成，并加载后执行。
        /// 因此你可以使用 IBehavior 对 HTML 进行读取，它会在验证之前执行。
        /// </summary>
        /// <param name="behaviors"></param>
        /// <returns></returns>
        public Staticizer AddBehavior(params IBehavior[] behaviors)
        {
            if (behaviors == null)
            {
                throw new ArgumentNullException("behaviors");
            }
            if (this.m_Behaviors == null)
            {
                this.m_Behaviors = new List<IBehavior>(behaviors);
            }
            else
            {
                this.m_Behaviors.AddRange(behaviors);
            }
            return this;
        }

        List<IValidation> m_Validations;


        /// <summary>
        /// 添加自定义验证规则。
        /// </summary>
        /// <param name="validations">自定义验证规则。</param>
        public Staticizer AddValidation(params  IValidation[] validations)
        {
            if (validations == null)
            {
                throw new ArgumentNullException("validations");
            }
            if (this.m_Validations == null)
            {
                this.m_Validations = new List<IValidation>(validations);
            }
            else
            {
                this.m_Validations.AddRange(validations);
            }
            return this;
        }

    }
}