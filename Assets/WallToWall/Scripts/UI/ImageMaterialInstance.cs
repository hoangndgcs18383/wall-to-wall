using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageMaterialInstance : MonoBehaviour
{
    public string keyword = "FADE_ON";
    private Image uiImage;

    private void Awake()
    {
        uiImage = GetComponent<Image>();
        uiImage.material = new Material(uiImage.material);
    }

    private void Start()
    {
        uiImage.material.DisableKeyword(keyword);
    }
}