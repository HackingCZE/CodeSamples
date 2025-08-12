using UnityEngine;

namespace DH.Core.Helpers.Utility
{
    public class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
    {
        public bool AutoUnParentOnAwake = true;

        protected override void InitializeSingleton()
        {
            if (AutoUnParentOnAwake)
            {
                transform.SetParent(null);
            }

            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                if (_instance != this)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}

