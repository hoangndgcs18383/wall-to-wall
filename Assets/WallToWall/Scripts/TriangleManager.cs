using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TriangleManager : MonoBehaviour
{
    public static TriangleManager Instance;

    public bool inManualAtStart;
    public float offset = 3f;

    public GameObject TriangleObj;

    public GameObject LeftWall;
    public GameObject RightWall;

    public List<Sprite> backgroundSprites;
    public SpriteRenderer background;

    float offsetLeft;
    float offsetRight = -0.7f;


    int NumberOfTriangles;

    [Space] [Range(0.5f, 2.0f)] public float scale = 1;


    [Space] [Range(0, 15)] public int NumberOfTriangles_Start;
    [Range(1, 15)] public int NumberOfTriangles_Max;
    [Range(1, 10)] public int TriangleCountUpScore;


    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        NumberOfTriangles = NumberOfTriangles_Start;

        offsetLeft = (LeftWall.transform.localScale.x / 2f);
        offsetRight = -(RightWall.transform.localScale.x / 2f);

        if (inManualAtStart)
        {
            StartCoroutine(CreateTrianglesStart("Left"));
            StartCoroutine(CreateTrianglesStart("Right"));
        }
        else
        {
            StartCoroutine(CreateTriangles("Left"));
            StartCoroutine(CreateTriangles("Right"));
        }

        _triangleRList.Clear();
        _triangleLList.Clear();
        _triangleStart.Clear();
    }

    public void WallTouched(string LeftOrRight)
    {
        DeleteTriangles(LeftOrRight);
        StartCoroutine(CreateTriangles(LeftOrRight));
    }

    public void StartGame()
    {
        ClearAllTriangles();
        StartCoroutine(CreateTriangles("Left"));
        StartCoroutine(CreateTriangles("Right"));
    }

    IEnumerator CreateTrianglesStart(string LeftOrRight)
    {
        yield return new WaitForSeconds(0.1f);
        if (LeftOrRight == "Left")
        {
            CreatePoolTriangle(LeftWall, TriangleObj, 3, offset);
            CreatePoolTriangle(LeftWall, TriangleObj, 3, 0f);
            CreatePoolTriangle(LeftWall, TriangleObj, 3, -offset);
        }
        else if (LeftOrRight == "Right")
        {
            CreatePoolTriangle(RightWall, TriangleObj, 3, offset);
            CreatePoolTriangle(RightWall, TriangleObj, 3, 0f);
            CreatePoolTriangle(RightWall, TriangleObj, 3, -offset);
        }

        yield return new WaitForSeconds(0.01f);
    }

    private void CreatePoolTriangle(GameObject wall, GameObject prefab, int i, float y)
    {
        PoolManager.Instance.CreateOrGetPool(prefab, i, (obj) =>
        {
            obj.transform.SetParent(wall.transform);
            obj.transform.position = new Vector2(wall.transform.position.x + offsetRight, y);
            obj.transform.rotation = wall.transform.rotation;
            SetScale(obj);
            obj.SetActive(true);
            _triangleStart.Add(obj);
        });
    }

    private List<GameObject> _triangleRList = new List<GameObject>();
    private List<GameObject> _triangleLList = new List<GameObject>();
    private List<GameObject> _triangleStart = new List<GameObject>();

    IEnumerator CreateTriangles(string LeftOrRight)
    {
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < NumberOfTriangles; i++) // Multiple triangles may appear in the same place.
        {
            int randomY = Random.Range(-6, 7);
            if (LeftOrRight == "Left")
            {
                PoolManager.Instance.CreateOrGetPool(TriangleObj, 5, (obj) =>
                {
                    obj.transform.SetParent(LeftWall.transform);
                    obj.transform.position = new Vector2(LeftWall.transform.position.x + offsetLeft, randomY * 1.5f);
                    obj.transform.rotation = LeftWall.transform.rotation;
                    SetScale(obj);
                    obj.SetActive(true);
                    _triangleLList.Add(obj);
                });
            }
            else if (LeftOrRight == "Right")
            {
                PoolManager.Instance.CreateOrGetPool(TriangleObj, 5, (obj) =>
                {
                    obj.transform.SetParent(RightWall.transform);
                    obj.transform.position = new Vector2(RightWall.transform.position.x + offsetRight, randomY * 1.5f);
                    obj.transform.rotation = RightWall.transform.rotation;
                    SetScale(obj);
                    obj.SetActive(true);
                    _triangleRList.Add(obj);
                });
            }

            yield return new WaitForSeconds(0.01f);
        }

        IncreaseNumberOfTriangles();
        yield break;
    }

    void SetScale(GameObject go)
    {
        return;
        go.transform.GetChild(0).transform.localScale = new Vector2(scale, scale);
        go.transform.GetChild(1).transform.localScale = new Vector2(scale, scale);
        go.transform.GetChild(2).transform.localScale = new Vector2(scale, scale);
    }


    void DeleteTriangles(string LeftOrRight)
    {
        if (LeftOrRight == "Left")
        {
            for (int i = 0; i < _triangleLList.Count; i++)
            {
                PoolManager.Instance.ReturnPool(TriangleObj, _triangleLList[i], callback: (go) =>
                {
                    go.GetComponent<Triangle>().TurnOff();
                    _triangleLList.RemoveAt(i);
                });
            }
        }

        if (LeftOrRight == "Right")
        {
            for (int i = 0; i < _triangleRList.Count; i++)
            {
                PoolManager.Instance.ReturnPool(TriangleObj, _triangleRList[i], callback: (go) =>
                {
                    go.GetComponent<Triangle>().TurnOff();
                    _triangleRList.RemoveAt(i);
                });
            }
        }
    }

    private void ClearAllTriangles()
    {
        for (int i = 0; i < _triangleStart.Count; i++)
        {
            PoolManager.Instance.ReturnPool(TriangleObj, _triangleStart[i],
                callback: (go) => { go.GetComponent<Triangle>().TurnOff(); });
        }
    }

    private int _currentBackgroundIndex = 0;

    //private bool _isBackgroundChanged = false;
    private int _currentScore = 0;

    void IncreaseNumberOfTriangles()
    {
        _currentScore = GameManager.Instance.score;

        if (_currentScore % 5 == 0)
        {
            _currentBackgroundIndex++;
            StartCoroutine(IEChangeBackgroundColor());
            if (backgroundSprites.Count <= _currentBackgroundIndex)
            {
                _currentBackgroundIndex = 0;
            }
        }

        if (NumberOfTriangles >= NumberOfTriangles_Max) return;
        NumberOfTriangles = _currentScore / TriangleCountUpScore + 1;
    }


    private IEnumerator IEChangeBackgroundColor()
    {
        background.material.EnableKeyword("DOODLE_ON");

        while (background.material.GetFloat("_RoundWaveStrength") < 1)
        {
            background.material.SetFloat("_RoundWaveStrength",
                background.material.GetFloat("_RoundWaveStrength") + 0.01f);
            yield return new WaitForSeconds(0.01f);
        }

        background.material.DisableKeyword("DOODLE_ON");
        background.sprite = backgroundSprites[_currentBackgroundIndex];
    }
}