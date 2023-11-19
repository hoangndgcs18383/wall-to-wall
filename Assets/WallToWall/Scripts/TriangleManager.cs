using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class TriangleManager : MonoBehaviour
{
    public static TriangleManager Instance;
    public PlayerConfig triangleConfig;
    public GameObject TriangleObj;
    public GameObject LeftWall;
    public GameObject RightWall;
    public SpriteRenderer background;
    private int NumberOfTriangles;
    private Dictionary<int, Sprite> _backgroundSprites = new Dictionary<int, Sprite>();

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _backgroundSprites.Clear();

        for (int i = 0; i < triangleConfig.backgroundKeys.Count; i++)
        {
            if (AddressablesManager.TryLoadAssetSync(BackgroundAddress.GetAddress(triangleConfig.backgroundKeys[i].key),
                    out Sprite sprite))
            {   
                _backgroundSprites.Add(i, sprite);
                Debug.Log($"Add background {i * triangleConfig.multipleScoreChangeBackgrounds}");
            };
        }

        background.sprite = _backgroundSprites[0];
        background.GetComponent<BackgroundScreenSize>().Validate();

        NumberOfTriangles = triangleConfig.numberOfTrianglesStart;

        triangleConfig.offsetLeft = (LeftWall.transform.localScale.x / 2f);
        triangleConfig.offsetRight = -(RightWall.transform.localScale.x / 2f);

        if (triangleConfig.inManualAtStart)
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
            CreatePoolTriangle(LeftWall, TriangleObj, 3, triangleConfig.offsetAtStart);
            CreatePoolTriangle(LeftWall, TriangleObj, 3, 0f);
            CreatePoolTriangle(LeftWall, TriangleObj, 3, -triangleConfig.offsetAtStart);
        }
        else if (LeftOrRight == "Right")
        {
            CreatePoolTriangle(RightWall, TriangleObj, 3, triangleConfig.offsetAtStart);
            CreatePoolTriangle(RightWall, TriangleObj, 3, 0f);
            CreatePoolTriangle(RightWall, TriangleObj, 3, -triangleConfig.offsetAtStart);
        }

        yield return new WaitForSeconds(0.01f);
    }

    private void CreatePoolTriangle(GameObject wall, GameObject prefab, int i, float y)
    {
        PoolManager.Instance.CreateOrGetPool(prefab, i, (obj) =>
        {
            obj.transform.SetParent(wall.transform);
            obj.transform.position = new Vector2(wall.transform.position.x + triangleConfig.offsetRight, y);
            obj.transform.rotation = wall.transform.rotation;
            obj.SetActive(true);
            _triangleStart.Add(obj);
        });
    }

    [SerializeField] private List<GameObject> _triangleRList = new List<GameObject>();
    [SerializeField] private List<GameObject> _triangleLList = new List<GameObject>();
    private List<GameObject> _triangleStart = new List<GameObject>();

    IEnumerator CreateTriangles(string LeftOrRight)
    {
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < NumberOfTriangles; i++) // Multiple triangles may appear in the same place.
        {
            int randomY = Random.Range(-6, 7);
            if (LeftOrRight == "Right")
            {
                PoolManager.Instance.CreateOrGetPool(TriangleObj, 1, (obj) =>
                {
                    obj.transform.SetParent(LeftWall.transform);
                    obj.transform.position = new Vector2(LeftWall.transform.position.x + triangleConfig.offsetLeft,
                        randomY * 1.5f);
                    obj.transform.rotation = LeftWall.transform.rotation;
                    obj.SetActive(true);
                    //_triangleLList.Add(obj);
                });
            }
            else if (LeftOrRight == "Left")
            {
                PoolManager.Instance.CreateOrGetPool(TriangleObj, 1, (obj) =>
                {
                    obj.transform.SetParent(RightWall.transform);
                    obj.transform.position = new Vector2(RightWall.transform.position.x + triangleConfig.offsetRight,
                        randomY * 1.5f);
                    obj.transform.rotation = RightWall.transform.rotation;
                    obj.SetActive(true);
                    //_triangleRList.Add(obj);
                });
            }

            yield return new WaitForSeconds(0.01f);
        }

        IncreaseNumberOfTriangles();
        yield break;
    }


    void DeleteTriangles(string LeftOrRight)
    {
        if (LeftOrRight == "Left")
        {
            foreach (Transform child in RightWall.transform)
            {
                if(!child.gameObject.activeSelf) continue; 
                PoolManager.Instance.ReturnPool(TriangleObj, child.gameObject, callback: (go) =>
                {
                    go.GetComponent<Triangle>().TurnOff();
                });
            }
        }
        else if (LeftOrRight == "Right")
        {
            foreach (Transform child in LeftWall.transform)
            {
                if(!child.gameObject.activeSelf) continue; 
                PoolManager.Instance.ReturnPool(TriangleObj, child.gameObject, callback: (go) =>
                {
                    go.GetComponent<Triangle>().TurnOff();
                });
            }
            /*for (int i = 0; i < _triangleRList.Count; i++)
            {
                PoolManager.Instance.ReturnPool(TriangleObj, _triangleRList[i], callback: (go) =>
                {
                    go.GetComponent<Triangle>().TurnOff();
                });
                _triangleRList.RemoveAt(i);
            }*/
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

        if (triangleConfig.backgroundKeys[_currentBackgroundIndex].score <= _currentScore)
        {
            _currentBackgroundIndex++;
            StartCoroutine(IEChangeBackgroundColor());
            if (triangleConfig.backgroundKeys.Count <= _currentBackgroundIndex)
            {
                for (int i = 0; i < triangleConfig.backgroundKeys.Count; i++)
                {
                    var triangleConfigBackgroundKey = triangleConfig.backgroundKeys[i];
                    triangleConfigBackgroundKey.score += triangleConfig.multipleScoreChangeBackgrounds;
                    triangleConfig.backgroundKeys[i] = triangleConfigBackgroundKey;
                }
                _currentBackgroundIndex = 0;
            }
        }

        /*if (_backgroundSprites.ContainsKey(_currentScore) && _currentScore != 0)
        {
            _currentBackgroundIndex++;
            StartCoroutine(IEChangeBackgroundColor());
            if (triangleConfig.backgroundKeys.Count <= _currentBackgroundIndex)
            {
                _currentBackgroundIndex = 0;
            }
        }*/

        if (NumberOfTriangles >= triangleConfig.numberOfTrianglesMax) return;
        NumberOfTriangles = _currentScore / triangleConfig.triangleCountUpScore + 1;
    }


    private IEnumerator IEChangeBackgroundColor()
    {
        background.material.SetFloat("_RoundWaveStrength", 0);

        while (background.material.GetFloat("_RoundWaveStrength") < 1)
        {
            background.material.SetFloat("_RoundWaveStrength",
                background.material.GetFloat("_RoundWaveStrength") + 0.1f);
            yield return new WaitForSeconds(0.01f);
        }

        background.material.SetFloat("_RoundWaveStrength", 0);
        background.sprite = _backgroundSprites[_currentBackgroundIndex];
    }
}