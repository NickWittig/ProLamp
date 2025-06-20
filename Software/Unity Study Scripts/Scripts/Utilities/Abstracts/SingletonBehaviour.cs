using UnityEngine;
using UnityEngine.Assertions;

public abstract class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                // Find an existing instance in the scene
                instance = FindObjectOfType<T>();
            }

            return instance;
        }
    }

    protected virtual void Awake(bool isIndestructable = true)
    {
        if (instance == null)
        {
            instance = (T)this;
            if (isIndestructable) DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Destroy any additional instances that might be created
            Destroy(gameObject);
        }
    }
}

