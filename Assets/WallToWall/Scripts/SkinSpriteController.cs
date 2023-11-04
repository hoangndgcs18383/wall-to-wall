using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkinSpriteController : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    private Material _material;
    
    void Start()
    {
        _material = GetComponent<Image>().material;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _material.DisableKeyword("DOODLE_ON");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _material.EnableKeyword("DOODLE_ON");
    }
}
