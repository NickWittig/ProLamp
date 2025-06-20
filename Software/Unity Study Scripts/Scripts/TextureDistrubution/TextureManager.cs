using UnityEngine;

public class TextureManager : SingletonBehaviour<TextureManager>
{

    [field: SerializeField] public GameObject local { get; private set; }
    [field: SerializeField] public GameObject remote { get; private set; }
    void Awake()
    {
        base.Awake(false);
    }

    private void OnValidate()
    {
        if (local != null)
        {
            if (local.GetComponent<ITextureProvider>() == null)
            {
                local = null;
            }
        }

        if (remote != null)
        {
            if (remote.GetComponent<ITextureProvider>() == null)
            {
                remote = null;
            }
        }
    }
}
