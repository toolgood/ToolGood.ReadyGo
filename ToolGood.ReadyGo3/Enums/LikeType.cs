using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo3
{
    public enum LikeType
    {
        /// <summary>
        /// 默认
        /// </summary>
        Default,
        /// <summary>
        /// 文本最后添加 %
        /// </summary>
        StartWith,
        /// <summary>
        /// 文本最前添加 %
        /// </summary>
        EndWith,
        /// <summary>
        /// 文本前后添加 %
        /// </summary>
        Contains
    }
}
