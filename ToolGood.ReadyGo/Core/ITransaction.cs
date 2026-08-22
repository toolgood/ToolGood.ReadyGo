using System;

namespace ToolGood.ReadyGo.NPoco
{
    public interface ITransaction : IDisposable
    {
        void Complete();
    }
}