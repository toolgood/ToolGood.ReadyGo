﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo3.Attributes
{
    /// <summary>
    /// 过滤Html XSS攻击
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class HtmlAttribute : Attribute
    {
    }
}