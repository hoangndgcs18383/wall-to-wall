using System;
using System.Collections;
using System.Collections.Generic;
using FreakyBall.Abilities;
using Hzeff.Events;
using MEC;
using UnityEngine;
using Random = UnityEngine.Random;

public enum PlayerState
{
    Idle,
    Ability,
    ChooseDirection,
    Input
}

public class BaseEntity : MonoBehaviour, IEntity
{
    //[SerializeField] private GameObject deadEffectPrefab; => This is not used
    /*[SerializeField] private GameObject wallBounceEffectPrefab;
    [SerializeField] private GameObject jumpEffectPrefab;
    [SerializeField] private GameObject touchEffectPrefab;*/

    public event Action<PlayerState> OnPlayerChangeState;

    public bool IsReceiveLive
    {
        get => isReceiveLive;
        set => isReceiveLive = value;
    }

    public PlayerState PlayerState
    {
        get => _playerState;
        set
        {
            if (AbilitySystem) AbilitySystem.OnPlayerChangeState(value, value.ToString());
            OnPlayerChangeState?.Invoke(value);
            _playerState = value;
        }
    }

    private bool isReceiveLive;

    private PlayerConfig _playerConfig;
    private Animator _deadAnimator;
    private Rigidbody2D _rigidbody2D;

    private Camera _mainCamera;
    private CameraShake _cameraShake;
    private SkinData _skinData;
    private bool _tutorialProcess;
    private int _deathCount;
    private bool _isDead;
    private Material _material;

    private PlayerState _playerState = PlayerState.Idle;
    private Vector2[] lineRendererPositions = new Vector2[2];
    private LineRenderer lineRenderer;
    protected AbilitySystem AbilitySystem;

    private EventBinding<PlayerCommandData> _playerCommandData;

    public void Initialize(PlayerConfig config, SkinData skinData)
    {
        _playerConfig = config;
        _skinData = skinData;
        _deadAnimator = GetComponentInChildren<Animator>(true);
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _material = GetComponent<SpriteRenderer>().material;

        ResetPlayerDefault();

        _mainCamera = Camera.main;
        if (_mainCamera != null) _cameraShake = _mainCamera.GetComponent<CameraShake>();
        _deathCount = SaveSystem.Instance.GetInt(PrefKeys.DeathCount);

        _playerCommandData = new EventBinding<PlayerCommandData>(OnPlayerCommand);
        EventDispatcher<PlayerCommandData>.Register(_playerCommandData);
    }

    public void SetLayer(int layer)
    {
        gameObject.layer = layer;
    }

    public void Dispose()
    {
        EventDispatcher<PlayerCommandData>.Unregister(_playerCommandData);
    }

    public void StopImmediate()
    {
        ResetPlayerDefault();
        PlayerState = PlayerState.Idle;
    }

    public void SetLineRendererToChooseDirection()
    {
        PlayerState = PlayerState.ChooseDirection;
        SetDefaultLineRenderer();
    }

    private void SetDefaultLineRenderer()
    {
        if (lineRenderer == null)
        {
            GameObject ob = new GameObject("LineRenderer");
            lineRenderer = ob.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }

        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.green;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
    }

    public void LineRendererToChooseDirection()
    {
        //TODO: Implement this

        lineRendererPositions[0] = transform.position;
        lineRenderer.SetPosition(0, lineRendererPositions[0]);

#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount <= 0) return;
        Vector3 touchPos = _mainCamera.ScreenToWorldPoint(Input.GetTouch(0).position);
        touchPos.z = 0;
        lineRendererPositions[1] = touchPos;

        lineRenderer.SetPosition(1, lineRendererPositions[1]);

        if (Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            ContinueGame();
            AddForcePlayerToLineRendererVector();
            lineRenderer.positionCount = 0;
            lineRendererPositions = new Vector2[2];
        }
#else
        Vector3 mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        lineRendererPositions[1] = mousePos;
        lineRenderer.SetPosition(1, lineRendererPositions[1]);

        if (Input.GetMouseButtonDown(0))
        {
            ContinueGame();
            AddForcePlayerToLineRendererVector();
            lineRenderer.positionCount = 0;
            lineRendererPositions = new Vector2[2];
        }
#endif


        //next input
    }

    private void AddForcePlayerToLineRendererVector()
    {
        //implement speed base on distance

        Vector2 distance = lineRendererPositions[1] - lineRendererPositions[0];
        float distanceMagnitude = distance.magnitude;
        float speed = distanceMagnitude * 0.5f;
        if (speed > 10) speed = 2;
        if (speed < 5) speed = 1;

        Vector2 direction = (lineRendererPositions[1] - lineRendererPositions[0]).normalized;
        _rigidbody2D.velocity = direction * _playerConfig.jumpSpeedX * speed;
        _rigidbody2D.isKinematic = false;
    }

    public void ContinueGame()
    {
        _rigidbody2D.isKinematic = false;
        PlayerState = PlayerState.Input;
    }

    private void Update()
    {
        if (!_rigidbody2D.gravityScale.Equals(_playerConfig.gravity) && !_rigidbody2D)
        {
            _rigidbody2D.gravityScale = _playerConfig.gravity;
        }

        if (!GameManager.Instance.IsStarted) return;

        if (_isDead) return;
        if (_tutorialProcess) return;


        switch (PlayerState)
        {
            case PlayerState.Idle:
                break;
            case PlayerState.Ability:
                break;
            case PlayerState.ChooseDirection:
                LineRendererToChooseDirection();
                break;
            case PlayerState.Input:
                UserInput();
                break;
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag(LayerKeys.WALL))
        {
            OnEnterWallCollision(other.gameObject.name, other.contacts[0].point);
        }

        if (other.gameObject.CompareTag(LayerKeys.TRIANGLE) && _isDead == false)
        {
            OnEnterTriangleCollision();
        }
    }

    #region Player

    protected virtual void OnEnterWallCollision(string collisionName, Vector3 contactPoint)
    {
        RandomSfxEnterWall();

        if (collisionName.Equals("Left") || collisionName.Equals("Right"))
        {
            GameManager.Instance.AddScore();
            TriangleManager.Instance.WallTouched(collisionName);
        }

        SpawnWallBounceEffect(collisionName, _skinData.key, contactPoint);
    }

    protected virtual void OnEnterTriangleCollision()
    {
        _isDead = true;
        GameManager.Instance.GameOver();
        StartCoroutine(_cameraShake.Shake(0.2f, 0.3f));
        ResetPlayerDefault();
    }

    protected virtual void SpawnWallBounceEffect(string colliderName, string skinKey, Vector3 contactPoint)
    {
        PoolManager.Instance.CreateOrGetPool(_playerConfig.wallBounceEffectPrefab, 3, (obj) =>
        {
            obj.transform.rotation = colliderName switch
            {
                "Left" => Quaternion.Euler(0, 0, 0),
                "Right" => Quaternion.Euler(0, 0, 180),
                "Top" => Quaternion.Euler(0, 0, -90),
                "Bottom" => Quaternion.Euler(0, 0, 90),
                _ => obj.transform.rotation
            };

            obj.transform.position = contactPoint;
            obj.SetActive(true);
            Animator animator = obj.GetComponentInChildren<Animator>();
            //Set animator state
            animator.SetTrigger(skinKey);
            StartCoroutine(ReturnPool(_playerConfig.wallBounceEffectPrefab, obj, isAutoDisable: true));
        });
    }

    protected virtual void RandomSfxEnterWall()
    {
        int random = Random.Range(0, 2);
        AudioManager.Instance.PlaySfx(random != 0 ? "JumpSFX_02" : "JumpSFX_01");
    }

    public virtual void StartGame()
    {
        AbilitySystem = GetComponent<AbilitySystem>();

        if (!TutorialManager.Instance.HadReleasedTutorial)
        {
            Timing.RunCoroutine(TutorialProcess());
        }

        _rigidbody2D.velocity = _rigidbody2D.velocity.x > 0
            ? new Vector2(_playerConfig.jumpSpeedX, _playerConfig.jumpSpeedY)
            : new Vector2(-_playerConfig.jumpSpeedX, _playerConfig.jumpSpeedY);

        _rigidbody2D.isKinematic = false;
        PlayerState = PlayerState.Input;
    }

    private bool UserInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            /*PoolManager.Instance.CreateOrGetPool(jumpEffectPrefab, 3, (obj) =>
            {
                obj.transform.position = transform.position;
                obj.SetActive(true);
                StartCoroutine(ReturnPool(jumpEffectPrefab, obj));
            });*/

            PoolManager.Instance.CreateOrGetPool(_playerConfig.touchEffectPrefab, 3, (obj) =>
            {
                var pos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                pos.z = 0;
                obj.transform.position = pos;
                obj.SetActive(true);
                StartCoroutine(ReturnPool(_playerConfig.touchEffectPrefab, obj, isAutoDisable: true));
            });

            //AudioManager.Instance.PlaySfx("TouchSFX");

            if (_rigidbody2D.velocity.x > 0)
            {
                _rigidbody2D.velocity = new Vector2(_playerConfig.jumpSpeedX, _playerConfig.jumpSpeedY);
            }
            else
            {
                _rigidbody2D.velocity = new Vector2(-_playerConfig.jumpSpeedX, _playerConfig.jumpSpeedY);
            }

            return true;
        }

        return false;
    }

    private IEnumerator<float> TutorialProcess()
    {
        yield return Timing.WaitForSeconds(1f);
        ResetPlayerDefault();
        _tutorialProcess = true;
    }

    private IEnumerator ReturnPool(GameObject prefab, GameObject obj, float delay = 0.5f, bool isAutoDisable = false,
        Action<GameObject> callback = null)
    {
        yield return PoolManager.Instance.ReturnPool(prefab, obj, delay, isAutoDisable, callback);
    }

    public virtual void SetPlayerPosition(Vector3 position)
    {
        transform.position = position;
    }

    public virtual void ResetPlayerDefault()
    {
        _rigidbody2D.velocity = new Vector2(0, 0);
        _rigidbody2D.isKinematic = true;
        _rigidbody2D.freezeRotation = true;
    }


    public virtual void OnPlayerCommand(PlayerCommandData playerCommandData)
    {
    }

    public void AddDeathCount()
    {
        _deathCount++;
        SaveSystem.Instance.SetInt(PrefKeys.DeathCount, _deathCount);
    }

    public Vector2 GetPosition()
    {
        return _rigidbody2D.velocity.normalized;
    }

    public void SetSkin(Sprite unlockSprite)
    {
        GetComponent<SpriteRenderer>().sprite = unlockSprite;
    }

    public void SetActiveSprite(bool b)
    {
        GetComponent<SpriteRenderer>().enabled = b;
    }

    public void PlayDeadAnimation()
    {
        _deadAnimator.gameObject.SetActive(true);
        _deadAnimator.SetTrigger(_skinData.key);
    }

    public void DisableAnimator()
    {
        _deadAnimator.gameObject.SetActive(false);
    }

    public Material GetMaterial()
    {
        return _material;
    }

    #endregion
}

public interface IEntity
{
    void Initialize(PlayerConfig config, SkinData skinData);
    void StartGame();
    void ContinueGame();
    void ResetPlayerDefault();
    void SetPlayerPosition(Vector3 position);
    Vector2 GetPosition();
    void AddDeathCount();
    void SetSkin(Sprite unlockSprite);
    void SetActiveSprite(bool b);
    void PlayDeadAnimation();
    void DisableAnimator();
    Material GetMaterial();
    void SetLayer(int layer);
    void SetLineRendererToChooseDirection();
    void StopImmediate();
    bool IsReceiveLive { get; set; }
    void Dispose();
}