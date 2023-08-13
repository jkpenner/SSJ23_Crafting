using UnityEngine;

namespace SSJ23_Crafting
{
    /// <summary>
    /// Base class for any MonoBehaviour that need to be singletons.
    /// </summary>
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance = null;

        public virtual void Awake()
        {
            if (!Exists())
            {
                SetInstance(this as T);
            }
        }

        protected virtual void OnDestroy()
        {
            if (Exists() && GetInstance() == this)
            {
                ClearInstance();
            }
        }

        /// <summary>
        /// Check if a singleton object is active.
        /// </summary>
        public static bool Exists()
        {
            return instance != null;
        }

        /// <summary>
        /// Check if the singleton instance is the active one.
        /// </summary>
        public bool IsActiveInstance()
        {
            return instance == this;
        }

        /// <summary>
        /// Gets the current active singleton object.
        /// </summary>
        public static T GetInstance()
        {
            return instance;
        }

        /// <summary>
        /// Tries to gets the current active singleton object. Returns
        /// if an instance was found.
        /// </summary>
        public static bool TryGetInstance(out T outInstance)
        {
            outInstance = GetInstance();
            return outInstance != null;
        }

        /// <summary>
        /// Attempts to find an instance of the singleton object or
        /// creates a new one if it does not exist. If a current
        /// instance is active will return that value.
        /// </summary>
        public static T FindOrCreateInstance()
        {
            if (!Exists())
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    instance = CreateInstance();
                }
            }

            return GetInstance();
        }

        /// <summary>
        /// Creates an instance of the singleton object.
        /// This does not assign the instance as active.
        /// </summary>
        private static T CreateInstance()
        {
            var manager = new GameObject(typeof(T).Name);
            return manager.AddComponent<T>();
        }

        /// <summary>
        /// Sets the value as the active instance of the Singleton.
        /// Will display error if a value is already assigned.
        /// </summary>
        protected static void SetInstance(T instance)
        {
            if (Singleton<T>.instance is null)
            {
                Singleton<T>.instance = instance;
            }
            else
            {
                Debug.LogErrorFormat("Attempting to assign an instance value to Singleton<{0}>, but one is already assigned", nameof(T));
            }
        }

        /// <summary>
        /// Clear the current instance of the singleton object.
        /// </summary>
        public static void ClearInstance()
        {
            instance = null;
        }
    }
}