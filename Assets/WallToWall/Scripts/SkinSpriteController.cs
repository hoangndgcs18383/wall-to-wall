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
        _material.SetFloat("_HandDrawnAmount", 0);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _material.SetFloat("_HandDrawnAmount", 10);
    }
}
