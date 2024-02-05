using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteRenderMaterialInstance : MonoBehaviour
{
    public string[] keywords;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.material = new Material(spriteRenderer.material);
    }

    private void Start()
    {
        if (keywords.Length > 0)
        {
            for (int i = 0; i < keywords.Length; i++)
            {
                spriteRenderer.material.DisableKeyword(keywords[i]);
            }
            
        }
    }
}