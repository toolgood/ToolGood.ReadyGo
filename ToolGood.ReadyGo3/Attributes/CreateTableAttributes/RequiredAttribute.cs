using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo3.Attributes
{
    public class RequiredAttribute : Attribute
    {
        public bool Required;

        public RequiredAttribute(bool required = true)
        {
            Required = required;
        }
    }
}
