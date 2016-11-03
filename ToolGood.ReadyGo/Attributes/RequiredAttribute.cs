using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo.Attributes
{
    public class RequiredAttribute : Attribute
    {
        public bool Required { get; private set; }

        public RequiredAttribute(bool required = true)
        {
            Required = required;
        }
    }
}
