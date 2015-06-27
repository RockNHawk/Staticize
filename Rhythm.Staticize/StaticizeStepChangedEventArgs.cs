using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rhythm.Staticize
{
    /// <summary>
    /// 关键状态变更事件参数
    /// </summary>
    public class StaticizeStepChangedEventArgs : System.EventArgs
    {
        /// <summary>
        /// 表示静态化过程关键步骤
        /// </summary>
        public StaticizeStep Step { get; set; }
    }
}
