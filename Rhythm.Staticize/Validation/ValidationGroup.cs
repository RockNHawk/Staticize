//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Rhythm.Staticize
//{
//    /// <summary>
//    /// 提供对被爬行页面的 HtmlDocument 验证支持。
//    /// </summary>
//    [Obsolete ]
//    public class ValidationGroup
//    {
//        List<IValidation> m_Validations;

//        /// <summary>
//        /// 使用lambda表达式或委托创建验证规则。
//        /// </summary>
//        /// <param name="documentValidation">一个委托，用于验证 HtmlDocument 是否符合规则，返回值为Boolean。</param>
//        /// <param name="errorMessage">验证不符合规则时的提示信息。</param>
//        public ValidationGroup Add(String name, System.Func<HtmlAgilityPack.HtmlDocument, bool> documentValidation, String errorMessage)
//        {
//            if (documentValidation == null)
//            {
//                throw new ArgumentNullException("documentValidation");
//            }
//            if (errorMessage == null)
//            {
//                throw new ArgumentNullException("errorMessage");
//            }
//            if (m_Validations == null)
//            {
//                m_Validations = new List<IValidation>();
//            }
//            m_Validations.Add(new ValidationDelegateTaken(name, documentValidation: documentValidation, errorMessage: errorMessage));
//            return this;
//        }

//        /// <summary>
//        /// 添加自定义验证规则。
//        /// </summary>
//        /// <param name="validations">自定义验证规则。</param>
//        public ValidationGroup Add(params  IValidation[] validations)
//        {
//            if (validations == null)
//            {
//                throw new ArgumentNullException("validations");
//            }
//            if (m_Validations == null)
//            {
//                m_Validations = new List<IValidation>();
//            }
//            m_Validations.AddRange(validations);
//            return this;
//        }


//        /// <summary>
//        /// 执行所有 HTML 检查
//        /// </summary>
//        /// <param name="status"></param>
//        /// <returns>HTML 检查结果。</returns>
//        public ValidationResult Validate(HtmlAgilityPack.HtmlDocument document, HtmlStaticizeContext status)
//        {
//            return m_Validations == null ? null : m_Validations.Validate(document, status);
//        }

//        public override string ToString()
//        {
//            if (m_Validations != null)
//            {
//                StringBuilder builder = new StringBuilder();
//                for (int i = 0; i < this.m_Validations.Count; i++)
//                {
//                    builder.AppendFormat("{0},", m_Validations[i].ToString());
//                }
//                return builder.ToString();
//            }
//            return base.ToString();
//        }

//        public IList<IValidation> Validations
//        {
//            get { return m_Validations; }
//        }

//    }
//}
