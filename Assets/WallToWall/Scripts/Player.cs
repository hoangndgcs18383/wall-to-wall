using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MEC;
using UnityEngine;
using UnityEngine.Serialization;
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

    [SerializeField] private Collider2D collider2D;
    [SerializeField] private Animator deadAnimator;

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
    private int _deathCount;
    private ISkill _currentSkill;
    private bool _canBeContinue;
    private SkinData _skinData;

    void Start()
    {
        _startPos = transform.position;
        cameraShake = Camera.main.GetComponent<CameraShake>();
        _camera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        GameManagerScript = GameObject.Find("GameManager").GetComponent<GameManager>();
        _material = GetComponent<SpriteRenderer>().material;

        _deathCount = SaveSystem.Instance.GetInt(PrefKeys.DeathCount);

        deadAnimator.gameObject.SetActive(false);
        StopPlayer();

        //var defaultEffect = DeadEffectObj;
        _skinData = SkinManager.Instance.GetCurrentSkin();
        DeadEffectObj = _skinData.effectData.deathEffect;
        //Debug.Log(SkinManager.Instance.GetCurrentKey());

        /*SetSkillByKey("Skin_0", "HydroSkill", _skinData, new HydroSkill(this));
        SetSkillByKey("Skin_1", "ShinySkill", _skinData);
        SetSkillByKey("Skin_5", "SoulSkill", _skinData, new SoulSkill(this));
        SetSkillByKey("Skin_2", "FrogSkill", _skinData, new FrogSkill(this));*/
    }

    private void SetSkillByKey(string key, string skillName, SkinData skinData, ISkillRelease skillRelease = null)
    {
        if (skinData.key == key)
        {
            _currentSkill = SkillManager.Instance.GetSkill(skillName);

            if (_currentSkill == null)
            {
                Debug.Log($"Load skill: {skillName}");
                _currentSkill = Resources.Load<SkillSO>($"Skills/{skillName}");
                _currentSkill.Initialize(skillRelease);
                Debug.Log("Load skill: " + _currentSkill.GetSkillDataConfig().NameDisplay);
                SkillManager.Instance.RegisterSkill($"{skillName}", _currentSkill);
                Debug.Log("Register skill: " + _currentSkill.GetSkillDataConfig().NameDisplay);
            }
            else
            {
                _currentSkill.Initialize(skillRelease);
            }

            SkillManager.Instance.SetCurrentSkill(_currentSkill);
        }
    }


    void Update()
    {
        SetConfigScaleGravity();
        if (isDead) return;
        if (!GameManagerScript.isStarted) return;

        if (_tutorialProcess) return;
        UserInput();

        if (Input.GetKeyDown(KeyCode.R))
        {
            UseSkill();
        }
    }

    private bool _tutorialProcess;
    private static readonly int Bounce = Animator.StringToHash("Bounce");
    private static readonly int Explode = Animator.StringToHash("Explode");

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

    private void UseSkill()
    {
        _currentSkill?.ReleaseSkill();
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
                //bounceAnimator.SetInteger(Animator.StringToHash("Bounce"), _skinData.bounceTrigger);

                GameObject.Find("GameManager").GetComponent<TriangleManager>().WallTouched(other.gameObject.name);
            }

            PoolManager.Instance.CreateOrGetPool(WallBounceEffectObj, 3, (obj) =>
            {
                // current direction is right

                if (other.gameObject.name == "Left")
                {
                    obj.transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                else if (other.gameObject.name == "Right")
                {
                    obj.transform.rotation = Quaternion.Euler(0, 0, 180);
                }
                else if (other.gameObject.name == "Top")
                {
                    obj.transform.rotation = Quaternion.Euler(0, 0, -90);
                }
                else if (other.gameObject.name == "Bottom")
                {
                    obj.transform.rotation = Quaternion.Euler(0, 0, 90);
                }

                obj.transform.position = other.contacts[0].point;
                obj.SetActive(true);
                Animator animator = obj.GetComponentInChildren<Animator>();
                //Set animator state
                animator.SetTrigger(_skinData.key);
                StartCoroutine(ReturnPool(WallBounceEffectObj, obj, isAutoDisable: true));
            });

            SetBackgroundColor();
        }

        if (other.gameObject.CompareTag("Triangle") && isDead == false)
        {
            if (_canBeContinue)
            {
                if (_currentSkill.GetSkillDataConfig().HasPassiveSkill) _currentSkill?.ReleaseSkill();
                Debug.Log($"Continue");
                return;
            }

            AudioManager.Instance.PlaySfx("DeadSFX");
            isDead = true;

            /*PoolManager.Instance.CreateOrGetPool(DeadEffectObj, 3, (obj) =>
            {
                obj.transform.position = transform.position;
                obj.SetActive(true);
                StartCoroutine(ReturnPool(DeadEffectObj, obj, isAutoDisable: true));
            });*/

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
        rb.freezeRotation = true;
    }

    public void ResetPlayer()
    {
        transform.position = _startPos;
        isDead = false;
        StopPlayer();
    }

    public Vector2 GetDirection()
    {
        return rb.velocity.normalized;
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

    public void PlayDeadAnimation()
    {
        deadAnimator.gameObject.SetActive(true);
        deadAnimator.SetTrigger(_skinData.key);
    }
    
    public void DisableAnimator()
    {
        deadAnimator.gameObject.SetActive(false);
    }

    public void SetActiveSprite(bool isActive)
    {
        GetComponent<SpriteRenderer>().enabled = isActive;
    }

    public bool IsDeadAnimationPlaying()
    {
        return deadAnimator.GetCurrentAnimatorStateInfo(0).IsName("Explode");
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
        _deathCount++;

        SaveSystem.Instance.SetInt(PrefKeys.DeathCount, _deathCount);
    }

    public void CanBeContinue(bool canBeContinue)
    {
        Debug.Log("CanBeContinue: " + canBeContinue);
        _canBeContinue = canBeContinue;
    }

    public void SetConfigScaleGravity()
    {
        if (rb.gravityScale != playerConfig.gravity && !rb)
        {
            rb.gravityScale = playerConfig.gravity;
        }
    }

    public Rigidbody2D GetRigidbody2D()
    {
        return rb;
    }

    public Material GetMaterial()
    {
        return _material;
    }
    
    public void SetLayer(int layer)
    {
        gameObject.layer = layer;
    }

}