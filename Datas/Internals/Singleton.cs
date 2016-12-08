using System;

namespace ToolGood.ReadyGo.Internals
{
    internal static class Singleton<T>
    {
        private static T _Instance;

        public static T Instance
        {
            get
            {
                if (_Instance == null) {
                    //_Instance = default(T);
                    //if (_Instance == null) {
                        _Instance = Activator.CreateInstance<T>();
                    //}
                }
                return _Instance;
            }
        }
    }
}