using System;
using System.Collections.Generic;
using System.Text;

namespace CSF.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class HelpAttribute : Attribute
    {
        public string Message { get; }
        
        public HelpAttribute(string helpMessage)
        {
            Message = helpMessage;
        }
    }
}
