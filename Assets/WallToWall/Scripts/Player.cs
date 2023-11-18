using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public PlayerConfig playerConfig;
    public GameObject WallBounceEffectObj;
    public GameObject DeadEffectObj;
    public GameObject JumpEffectObj;
    public CameraShake cameraShake;
    public SpriteRenderer background;

    private Vector3 _startPos;

    GameManager GameManagerScript;
    Rigidbody2D rb;


    [HideInInspector] public bool isDead = false;
    bool isFirstTouch = true;


    /*[Space] public int JumpSpeed_X;
    public int JumpSpeed_Y;

    public int Gravity;*/

    private Material _material;

    void Start()
    {
        _startPos = transform.position;
        cameraShake = Camera.main.GetComponent<CameraShake>();

        rb = GetComponent<Rigidbody2D>();
        GameManagerScript = GameObject.Find("GameManager").GetComponent<GameManager>();
        _material = GetComponent<SpriteRenderer>().material;
        StopPlayer();
    }


    void Update()
    {
        if (rb.gravityScale != playerConfig.gravity && !rb)
        {
            rb.gravityScale = playerConfig.gravity;
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

            if (rb.velocity.x > 0)
            {
                rb.velocity = new Vector2(playerConfig.jumpSpeedX, playerConfig.jumpSpeedY);
            }
            else
            {
                rb.velocity = new Vector2(-playerConfig.jumpSpeedX, playerConfig.jumpSpeedY);
            }
        }
    }


    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            int random = Random.Range(0, 2);
            AudioManager.Instance.PlaySfx(random != 0 ? "JumpSFX_02" : "JumpSFX_01");

            if (other.gameObject.name == "Left" || other.gameObject.name == "Right")
            {
                GameManagerScript.addScore();


                GameObject.Find("GameManager").GetComponent<TriangleManager>().WallTouched(other.gameObject.name);
            }

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
            AudioManager.Instance.PlaySfx("DeadSFX");
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
        return;
        background.material.SetColor("_InnerOutlineColor",
            Color.HSVToRGB(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
        float random = Random.value;
        background.material.SetFloat("_InnerOutlineAlpha", random);

        //_material.SetColor("_InnerOutlineColor", );

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