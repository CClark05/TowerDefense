using System;
using UnityEngine;

public class WhiteOutlineEffect : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Material whiteOutlineMat;
    private Material originalMat;
    private MaterialPropertyBlock mpb;
    private void Awake()
    {
        if (sr == null)
            sr = GetComponent<SpriteRenderer>();
        originalMat = sr.sharedMaterial;
        mpb = new MaterialPropertyBlock();
    }

    public void ToggleWhiteOutline(bool enabled)
    {
        if (enabled)
        {
            sr.sharedMaterial = whiteOutlineMat;
            sr.GetPropertyBlock(mpb);
            mpb.SetTexture("_MainTex", sr.sprite.texture);
            sr.SetPropertyBlock(mpb);
            return;
        }
        sr.sharedMaterial = originalMat;
        sr.SetPropertyBlock(null);
    }
    
}
