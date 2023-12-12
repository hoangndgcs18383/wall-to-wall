using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MEC;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public PlayerConfig playerConfig;
    public GameObject WallBounceEffectObj;
    public GameObject DeadEffectObj;
    public GameObject JumpEffectObj;
    public GameObject TouchEffectObj;
    public CameraShake cameraShake;
    public SpriteRenderer background;

    private Vector3 _startPos;

    GameManager GameManagerScript;
    Rigidbody2D rb;


    [HideInInspector] public bool isDead = false;
    private Camera _camera;
    bool isFirstTouch = true;


    /*[Space] public int JumpSpeed_X;
    public int JumpSpeed_Y;

    public int Gravity;*/

    private Material _material;

    void Start()
    {
        _startPos = transform.position;
        cameraShake = Camera.main.GetComponent<CameraShake>();
        _camera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        GameManagerScript = GameObject.Find("GameManager").GetComponent<GameManager>();
        _material = GetComponent<SpriteRenderer>().material;
        StopPlayer();

        //var defaultEffect = DeadEffectObj;
        SkinData skinData = SkinManager.Instance.GetCurrentSkin();
        DeadEffectObj = skinData.effectData.deathEffect;
        //Debug.Log(SkinManager.Instance.GetCurrentKey());
    }


    void Update()
    {
        if (rb.gravityScale != playerConfig.gravity && !rb)
        {
            rb.gravityScale = playerConfig.gravity;
        }

        if (isDead) return;
        if (!GameManagerScript.isStarted) return;

        if (_tutorialProcess) return;
        UserInput();
    }
    
    private bool _tutorialProcess;

    public IEnumerator<float> TutorialProcess()
    {
        yield return Timing.WaitForSeconds(1f);
        StopPlayerForTutorial();
        _tutorialProcess = true;
    }

    private void StopPlayerForTutorial()
    {
        rb.velocity = new Vector2(0, 0);
        rb.isKinematic = true;
    }

    private bool UserInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PoolManager.Instance.CreateOrGetPool(JumpEffectObj, 3, (obj) =>
            {
                obj.transform.position = transform.position;
                obj.SetActive(true);
                StartCoroutine(ReturnPool(JumpEffectObj, obj));
            });

            PoolManager.Instance.CreateOrGetPool(TouchEffectObj, 3, (obj) =>
            {
                var pos = _camera.ScreenToWorldPoint(Input.mousePosition);
                pos.z = 0;
                obj.transform.position = pos;
                obj.SetActive(true);
                StartCoroutine(ReturnPool(TouchEffectObj, obj, isAutoDisable: true));
            });

            //AudioManager.Instance.PlaySfx("TouchSFX");

            if (rb.velocity.x > 0)
            {
                rb.velocity = new Vector2(playerConfig.jumpSpeedX, playerConfig.jumpSpeedY);
            }
            else
            {
                rb.velocity = new Vector2(-playerConfig.jumpSpeedX, playerConfig.jumpSpeedY);
            }

            return true;
        }

        return false;
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
        if (!TutorialManager.Instance.HadReleasedTutorial)
        {
            Timing.RunCoroutine(TutorialProcess());
        }

        if (rb.velocity.x > 0)
        {
            rb.velocity = new Vector2(playerConfig.jumpSpeedX, playerConfig.jumpSpeedY);
        }
        else
        {
            rb.velocity = new Vector2(-playerConfig.jumpSpeedX, playerConfig.jumpSpeedY);
        }

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