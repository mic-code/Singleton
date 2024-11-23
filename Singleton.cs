using UnityEngine;

namespace Overimagined.Common
{
    public abstract class Singleton<T> where T : new()
    {
        protected static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                    Init();

                return _instance;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        protected static void ResetSubsystem()
        {
            Debug.Log("reset");
            _instance = default;
        }

        public static void Init()
        {
            _instance = new T();

            if (typeof(T) is Singleton<T> c)
                c.OnInit();
        }

        public abstract void OnInit();
    }

    public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<T>();

                return _instance;
            }
        }

        public virtual void Init() => _instance = Instance;
    }

    public abstract class SingletonMonoManagerBase : MonoBehaviour
    {
        protected static bool reset;
        protected static bool isDestroying;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        protected static void ResetSubsystem()
        {
            reset = true;
            isDestroying = false;
        }

        void OnDestroy()
        {
            isDestroying = true;
        }
    }

    public class SingletonMonoManager<T> : SingletonMonoManagerBase where T : SingletonMonoManager<T>
    {
        protected static T _instance;

        public static T Instance
        {
            get
            {
                if (reset || _instance == null)
                    _instance = FindObjectOfType<T>();

                if (reset || _instance == null)
                    _instance = CreateManager();

                reset = false;

                return _instance;
            }
        }

        public static T CreateManager()
        {
            if (isDestroying)
                return null;

            GameObject go;
            var prefab = Resources.Load(typeof(T).Name);
            if (prefab != null)
                go = Instantiate(prefab) as GameObject;
            else
                go = new GameObject("[" + typeof(T).Name + "]");

            go.name = go.name.Replace("(Clone)", "");

            if (go.GetComponent<T>() == null)
                return go.AddComponent<T>();
            else
                return go.GetComponent<T>();
        }

        public static void Boostrap()
        {
            _ = Instance;
            Instance.Init();
        }

        protected virtual void Init() { }
    }
}