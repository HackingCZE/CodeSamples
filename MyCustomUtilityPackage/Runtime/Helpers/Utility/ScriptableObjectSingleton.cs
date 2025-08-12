using UnityEngine;

namespace DH.Core.Helpers.Utility
{
    public class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObject
    {
        private static T _instance;
        public static T GetInstance()
        {
            if(_instance == null)
            {
                _instance = Resources.Load("ScriptableObjects/" + typeof(T).Name) as T;
            }
            return _instance;
        }
    }
}

