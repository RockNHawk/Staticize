using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rhythm.Staticize
{
    /// <summary>
    /// 网页静态化后的验证结果
    /// </summary>
    public class ValidationResult
    {
        public ValidationResult()
        {
            //Errors = new Dictionary<String, String>();
        }

        public int Id { get; set; }

        /// <summary>
        /// 链接地址（绝对路径）
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// 校验的类型
        /// </summary>
        public virtual ValidationType ValidationType { get; set; }

        /// <summary>
        /// 校验的短标题，用于在用户界面显示
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// 校验的结果信息，会在用户界面显示
        /// </summary>
        public virtual string Message { get; set; }

        /// <summary>
        /// 有的校验可能会产生异常，此属性用于记录详细异常信息。
        /// </summary>
        public virtual System.Exception Exception { get; set; }


        //public IDictionary<String, String> Errors { get; set; }

        //public bool IsValid
        //{
        //    get
        //    {
        //        var error = this.Errors;
        //        return (error == null || error.Count == 0);
        //    }
        //}

        //public void AddError(String name, String errorMessage)
        //{
        //    String existsMessage;
        //    if (Errors.TryGetValue(name, out existsMessage))
        //    {
        //        Errors[name] = String.Format("{0}\r\n{1}", existsMessage, errorMessage);
        //    }
        //    else
        //    {
        //        Errors.Add(name, errorMessage);
        //    }
        //}

        //public void RemoveError(String name)
        //{
        //    Errors.Remove(name);
        //}

        //public override string ToString()
        //{
        //    var errors = this.Errors;
        //    if (errors != null && errors.Count > 0)
        //    {
        //        System.Text.StringBuilder builder = new System.Text.StringBuilder(errors.Count * 20);
        //        builder.AppendFormat("以下是对页面{0}的验证结果：\r\n\r\n", Uri);
        //        foreach (var name in errors.Keys)
        //        {
        //            String message = errors[name];
        //            builder.AppendFormat("验证[{0}]不通过:\r\n{1}\r\n", name, message);
        //        }
        //        return builder.ToString();
        //    }
        //    return "";
        //}

        public override string ToString()
        {
            if (Message != null && Message.Length > 0)
            {
                return string.Format("验证[{0}]不通过:\r\n{1}\r\n", Name, Message);
            }
            return "";
        }


    }
}
