using Sirenix.OdinInspector;
using UnityEngine;

public class BackgroundScreenSize : MonoBehaviour
{
    float mapX = 100.0f;
    float mapY = 100.0f;
    
    public float offset;

    public GameObject Wall_Left;
    public GameObject Wall_Right;
    public GameObject Wall_Top;
    public GameObject Wall_Bottom;

    [Space]
    public bool removeWall;
    [Range(1, 5)]
    public float wallThickness;
    float positionOffset;

    //43.5 20 
    
    [SerializeField] private SpriteRenderer background;

    [Button("Validate")]
    void Awake()
    {
        float vertExtent = Camera.main.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;
        background.size = new Vector2(horzExtent * 2, vertExtent * 2);
    }
}