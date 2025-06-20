using UnityEngine;

public interface ITextureTransformer
{
    public void Apply(ref Texture2D texture2D);
}