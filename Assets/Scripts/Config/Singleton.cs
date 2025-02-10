using System;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static Dictionary<Type, MonoBehaviour> instances = new Dictionary<Type, MonoBehaviour>();

    public static T Instance
    {
        get
        {
            Type type = typeof(T);

            if (instances.ContainsKey(type))
            {
                return instances[type] as T;
            }

            // Nếu không tìm thấy, tìm trong scene
            T foundInstance = FindAnyObjectByType<T>();
            if (foundInstance == null)
            {
                Debug.LogWarning($"No instance of Singleton {type} found.");
                return null;
            }

            // Lưu trữ đối tượng tìm được vào dictionary
            (foundInstance as Singleton<T>)?.RegisterInstance();
            return foundInstance;
        }
    }

    protected virtual void Awake()
    {
        // Đăng ký instance khi đối tượng được tạo
        RegisterInstance();
    }

    private void RegisterInstance()
    {
        Type type = typeof(T);

        if (!instances.ContainsKey(type))
        {
            instances[type] = this;

            // Đảm bảo đối tượng không bị hủy khi chuyển scene
            DontDestroyOnLoad(gameObject);
        }
        else if (instances[type] != this)
        {
            Debug.LogWarning($"Multiple instances of Singleton {type} found. Destroying extra instance on {gameObject.name}.");
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        // Xóa instance khi đối tượng bị hủy
        Type type = typeof(T);
        if (instances.ContainsKey(type) && instances[type] == this)
        {
            instances.Remove(type);
        }
    }
}
