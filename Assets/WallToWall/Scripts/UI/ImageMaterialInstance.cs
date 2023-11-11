using UnityEngine;
using UnityEngine.UI;

public class ImageMaterialInstance : MonoBehaviour
{
    private void Awake()
    {
        Image uiImage = GetComponent<Image>();
        uiImage.material = new Material(uiImage.material);
    }
}
