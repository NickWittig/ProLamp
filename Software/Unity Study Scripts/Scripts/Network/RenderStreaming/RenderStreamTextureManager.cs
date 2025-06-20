using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderStreamTextureManager : SingletonBehaviour<RenderStreamTextureManager>
{

    [field: SerializeField] public RenderStreamTextureProvider local { get; private set; }
    [field: SerializeField] public RenderStreamTextureProvider remote { get; private set; }
    void Awake()
    {
        base.Awake(false);
    }

}
