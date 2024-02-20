using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Toolbox
{
    public abstract class PersistentMonoBehaviourSingleton<T> : MonoBehaviourSingleton<T> where T : MonoBehaviour
    {
		protected override void Awake()
		{
            base.Awake();

            DontDestroyOnLoad(this.gameObject);    
        }
	}
    public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static bool Quitting { get; private set; }

        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance != null) return instance;
                else
                {
                    T[] instances = FindObjectsOfType<T>(includeInactive: false).Where(i => i.enabled).ToArray();

                    if (instances.Length > 1)
                    {
                        for (var i = 1; i < instances.Length; i++)
                        {
                            Destroy(instances[i]);
                        }

                        Debug.LogWarning($"{instances.Length} instances of {typeof(T)} found. Extra instances have been destroyed.");

                        return instance = instances[0];   
                    }
                    else if (instances.Length == 0)
                    {
                        Debug.LogWarning($"No instances of {typeof(T)} found. An instance has been created");

                        return instance = new GameObject($"{typeof(T)}").AddComponent<T>();   
                    }
                    else // exactly 1 instance found
                    {
                        return instance = instances[0];
                    }
                }
            }
        }

        ///<summary> description of element </summary>  
        protected virtual void Awake()
        {
            if(!this.enabled || instance != null && instance != this)
            {
                Destroy(this);
            }
            else
            {
                instance = this as T;
            }

            OnAwake();
        }

        protected virtual void OnAwake() { }

        private void OnApplicationQuit()
        {
            Quitting = true;
        }
    }
}