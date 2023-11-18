
using UnityEngine;

public class BackgroundScreenSize : MonoBehaviour
{
    [SerializeField] private SpriteRenderer background;
    public void Validate()
    {
        float vertExtent = Camera.main.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;
        background.size = new Vector2(horzExtent * 2, vertExtent * 2);
    }
}