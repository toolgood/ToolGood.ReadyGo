using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo3.DataCentxt.Interfaces
{
    public interface IPrecondition<T> where T : class
    {
        T IfTrue(bool b);
        T IfFalse(bool b);

        T IfSet(string txt);
        T IfNotSet(string txt);

        T IfNullOrEmpty(string value);
        T IfNullOrWhiteSpace(string value);

        T IfNull(object obj);
        T IfNotNull(object obj);

    }

    public interface IPrecondition
    {
        void IfTrue(bool b);
        void IfFalse(bool b);

        void IfSet(string txt);
        void IfNotSet(string txt);

        void IfNullOrEmpty(string value);
        void IfNullOrWhiteSpace(string value);

        void IfNull(object obj);
        void IfNotNull(object obj);

    }
}
