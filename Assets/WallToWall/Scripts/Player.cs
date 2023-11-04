using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public GameObject WallBounceEffectObj;
    public GameObject DeadEffectObj;
    public GameObject JumpEffectObj;
    public CameraShake cameraShake;

    private Vector3 _startPos;

    GameManager GameManagerScript;
    Rigidbody2D rb;


    [HideInInspector] public bool isDead = false;
    bool isFirstTouch = true;


    float hueValue;

    AudioSource source;
    [Space] public AudioClip jumpClip;
    public AudioClip deadClip;

    [Space] public int JumpSpeed_X;
    public int JumpSpeed_Y;

    public int Gravity;

    private Material _material;

    void Start()
    {
        _startPos = transform.position;
        cameraShake = Camera.main.GetComponent<CameraShake>();
        source = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        GameManagerScript = GameObject.Find("GameManager").GetComponent<GameManager>();
        _material = GetComponent<SpriteRenderer>().material;

        hueValue = Random.Range(0, 10) / 10.0f;

        SetBackgroundColor();
        StopPlayer();
    }


    void Update()
    {
        if (rb.gravityScale != Gravity && !rb)
        {
            rb.gravityScale = Gravity;
        }

        if (isDead) return;
        if (!GameManagerScript.isStarted) return;

        if (Input.GetMouseButtonDown(0))
        {
            PoolManager.Instance.CreateOrGetPool(JumpEffectObj, 3, (obj) =>
            {
                obj.transform.position = transform.position;
                obj.SetActive(true);
                StartCoroutine(ReturnPool(JumpEffectObj, obj));
            });

            source.PlayOneShot(jumpClip, 1);

            if (rb.velocity.x > 0)
            {
                rb.velocity = new Vector2(JumpSpeed_X, JumpSpeed_Y);
            }
            else
            {
                rb.velocity = new Vector2(-JumpSpeed_X, JumpSpeed_Y);
            }
        }
    }


    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            source.PlayOneShot(jumpClip, 1);

            if (other.gameObject.name == "Left" || other.gameObject.name == "Right")
            {
                GameManagerScript.addScore();


                GameObject.Find("GameManager").GetComponent<TriangleManager>().WallTouched(other.gameObject.name);
            }

            /*GameObject effectObj = Instantiate(WallBounceEffectObj, other.contacts[0].point, Quaternion.identity);
            Destroy(effectObj, 0.5f);*/

            PoolManager.Instance.CreateOrGetPool(WallBounceEffectObj, 3, (obj) =>
            {
                obj.transform.position = other.contacts[0].point;
                obj.SetActive(true);
                StartCoroutine(ReturnPool(WallBounceEffectObj, obj, isAutoDisable: true));
            });

            SetBackgroundColor();
        }

        if (other.gameObject.CompareTag("Triangle") && isDead == false)
        {
            source.PlayOneShot(deadClip, 1);
            isDead = true;

            PoolManager.Instance.CreateOrGetPool(DeadEffectObj, 3, (obj) =>
            {
                obj.transform.position = transform.position;
                obj.SetActive(true);
                StartCoroutine(ReturnPool(DeadEffectObj, obj, isAutoDisable: true));
            });

            GameManagerScript.Gameover();

            StartCoroutine(cameraShake.Shake(0.2f, 0.3f));

            StopPlayer();
        }
    }

    private IEnumerator ReturnPool(GameObject prefab, GameObject obj, float delay = 0.5f, bool isAutoDisable = false,
        Action<GameObject> callback = null)
    {
        yield return PoolManager.Instance.ReturnPool(prefab, obj, delay, isAutoDisable, callback);
    }

    public void StartPlayer()
    {
        rb.velocity = new Vector2(-1, 0);
        rb.isKinematic = false;
        // isStarted = true;
    }


    void StopPlayer()
    {
        rb.velocity = new Vector2(0, 0);
        rb.isKinematic = true;
    }

    public void ResetPlayer()
    {
        transform.position = _startPos;
        isDead = false;
        StopPlayer();
    }


    void SetBackgroundColor()
    {
        /*hueValue += 0.1f;
        if (hueValue >= 1)
        {
            hueValue = 0;
        }

        // Camera.main.backgroundColor = Color.HSVToRGB(hueValue, 0.5f, 0.3f);
        Camera.main.backgroundColor = Color.HSVToRGB(hueValue, 0.6f, 0.8f);*/
    }
    
    public IEnumerator IEDeadAnimation()
    {
        _material.EnableKeyword("ROUNDWAVEUV_ON");
        
        while (_material.GetFloat("_RoundWaveStrength") < 1)
        {
            _material.SetFloat("_RoundWaveStrength", _material.GetFloat("_RoundWaveStrength") + 0.01f);
            yield return new WaitForSecondsRealtime(0.01f);
        }
        
        gameObject.SetActive(false);
        _material.DisableKeyword("ROUNDWAVEUV_ON");
        yield return new WaitForSecondsRealtime(0.1f);
    }
}