using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteRenderMaterialInstance : MonoBehaviour
{
    public string keyword = "FADE_ON";
    private SpriteRenderer  spriteRenderer;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.material = new Material(spriteRenderer.material);
    }

    private void Start()
    {
        spriteRenderer.material.DisableKeyword(keyword);
    }
}
