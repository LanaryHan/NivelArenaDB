using UnityEngine;

namespace Common
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static readonly object _lock = new object();

        private static bool _applicationIsQuitting = false;
        private static bool _isDestroyingManually = false;

        public static T Instance
        {
            get
            {
                if (_applicationIsQuitting)
                {
                    Debug.LogWarning($"[Singleton] Instance of {typeof(T)} is already destroyed. Returning null.");
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindFirstObjectByType<T>();

                        if (FindObjectsByType<T>(FindObjectsSortMode.None).Length > 1)
                        {
                            Debug.LogError($"[Singleton] More than one instance of {typeof(T)} found!");
                            return _instance;
                        }

                        if (_instance == null)
                        {
                            GameObject singletonObject = new GameObject();
                            _instance = singletonObject.AddComponent<T>();
                            singletonObject.name = $"{typeof(T)} (Singleton)";
                            DontDestroyOnLoad(singletonObject);
                        }
                    }

                    return _instance;
                }
            }
        }

        public static bool HasInstance => _instance != null;

        /// <summary>
        /// 手动销毁当前单例。
        /// 适合在卸载场景、退出某个模块时清理 DontDestroyOnLoad 对象。
        /// </summary>
        public static void DestroyInstance()
        {
            if (_instance == null)
            {
                return;
            }

            _isDestroyingManually = true;

            GameObject instanceObject = _instance.gameObject;
            _instance = null;

            if (Application.isPlaying)
            {
                Destroy(instanceObject);
            }
            else
            {
                DestroyImmediate(instanceObject);
            }

            _isDestroyingManually = false;
        }

        /// <summary>
        /// 手动销毁当前单例，并且之后不允许重新创建。
        /// 一般只在彻底退出系统时使用。
        /// </summary>
        public static void DestroyInstanceAndDisableCreate()
        {
            _applicationIsQuitting = true;
            DestroyInstance();
        }

        protected virtual void Awake()
        {
            if (_applicationIsQuitting)
            {
                Destroy(gameObject);
                return;
            }

            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        protected virtual void OnApplicationQuit()
        {
            _applicationIsQuitting = true;
        }
    }
}