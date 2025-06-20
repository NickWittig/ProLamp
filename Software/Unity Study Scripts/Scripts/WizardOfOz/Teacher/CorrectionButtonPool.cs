using System.Collections;
using UnityEngine;


public class CorrectionButtonPool : ObjectPoolBehaviour
{
   
    
    public static CorrectionButtonPool Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Start()
    {
        base.Initialize();
    }
}
