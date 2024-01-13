using System;
using System.Collections.Generic;
using DG.Tweening;
using MEC;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPanel : BaseScreen
{
    [SerializeField] private RectTransform handRectTransform;
    [SerializeField] private RectTransform pointArrowRectTransform;
    [SerializeField] private RectTransform thornRectTransform;
    [SerializeField] private GameObject mainContent;
    [SerializeField] private GameObject tapEffect;
    [SerializeField] private Image inEffect;
    [SerializeField] private Image hydroImage;
    [SerializeField] private TMP_Text hydroText;
    [SerializeField] private TMP_Text pointText;
    [SerializeField] private GameObject frameText;
    [SerializeField] private GameObject arrow;
    [SerializeField] private Image lineTutorial;
    [SerializeField] private Image effectTouchWall;
    [SerializeField] private Image hydroDeadImage;
    [SerializeField] private Image pointGreenImage;
    [SerializeField] private Image deadEffectImage;
    [SerializeField] private Image getReadyImage;

    [SerializeField] private Image[] hydroObjects;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Ease ease = Ease.OutBack;
    [SerializeField] private float jumpPower = 0.5f;

    private Vector3 _startHydroPos;
    private Sequence sequence;

    private int _currentStep = 0;
    private bool _isCanNextStep = false;

    private bool _nextTap = false;
    private Action _completeCallback;

    private bool _isComplete = false;

    //private static readonly int PinchUvAmount = Shader.PropertyToID("_PinchUvAmount");
    //private int _maxStep = 5;

    [Button]
    public void StartAnim()
    {
        //Timing.RunCoroutine(IEStartAnim());
        _isComplete = false;
        _currentStep = 0;
        _isCanNextStep = false;
        Timing.RunCoroutine(StepOne());
    }

    public override void Show(IUIData data = null)
    {
        base.Show(data);
        _currentStep = 0;
        _isCanNextStep = false;
        mainContent.SetActive(true);
        Timing.RunCoroutine(StepOne());
    }

    public void RegisterCompleteCallback(Action callback)
    {
        _completeCallback = callback;
    }

    private void Update()
    {
        if (_isComplete) return;
        if (!_isCanNextStep)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (_currentStep == 1)
                {
                    _nextTap = true;
                }
            }

            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (_currentStep == 0)
            {
                _currentStep++;
                _isCanNextStep = false;
                Timing.RunCoroutine(StepTwo());
            }
            else if (_currentStep == 1)
            {
                _currentStep++;
                _isCanNextStep = false;
                Timing.RunCoroutine(StepThree());
            }
            else if (_currentStep == 2)
            {
                _currentStep++;
                _isCanNextStep = false;
                Timing.RunCoroutine(StepFour());
            }
            else if (_currentStep == 3)
            {
                _currentStep++;
                _isCanNextStep = false;
                Timing.RunCoroutine(StepFive());
            }
            /*else if (_currentStep == 4)
            {
                _currentStep++;
                _isCanNextStep = false;
                Timing.RunCoroutine(StepFive());
            }*/
        }
    }

    /*public IEnumerator<float> IEStartAnim()
    {
        yield return Timing.WaitUntilDone(StepOne());
        yield return Timing.WaitUntilDone(StepTwo());
        yield return Timing.WaitUntilDone(StepThree());
        yield return Timing.WaitUntilDone(StepFour());
        yield return Timing.WaitUntilDone(StepFive());
    }*/

    private IEnumerator<float> StepOne()
    {
        for (int i = 1; i < hydroObjects.Length; i++)
        {
            hydroObjects[i].DOFade(0f, 0);
        }

        pointGreenImage.DOFade(0, 0);
        deadEffectImage.DOFade(0, 0);
        hydroDeadImage.DOFade(0, 0);
        effectTouchWall.DOFade(0, 0);
        getReadyImage.DOFade(0, 0);
        hydroObjects[0].DOFade(1, 0);
        inEffect.DOFade(0f, 0f);

        var localPosition = thornRectTransform.localPosition;
        localPosition = new Vector3(localPosition.x + 100f, localPosition.y, localPosition.z);
        thornRectTransform.localPosition = localPosition;
        frameText.SetActive(false);
        arrow.SetActive(false);
        pointArrowRectTransform.gameObject.SetActive(false);
        pointText.SetText(String.Empty);
        lineTutorial.gameObject.SetActive(false);
        handRectTransform.gameObject.SetActive(false);

        _startHydroPos = hydroImage.rectTransform.anchoredPosition;
        _startHydroPos.x *= -1;
        hydroImage.rectTransform.anchoredPosition = _startHydroPos;
        yield return Timing.WaitForSeconds(1f);
        _startHydroPos.x *= -1;
        hydroImage.rectTransform.DOAnchorPos(_startHydroPos, 0.5f).SetEase(Ease.InBack);
        yield return Timing.WaitForSeconds(0.5f);
        frameText.SetActive(true);
        yield return Timing.WaitUntilDone(IESetText("Tap on the screen to make \"HYDROs\" move"));
        arrow.SetActive(true);
        yield return Timing.WaitForSeconds(1f);
        _isCanNextStep = true;
    }

    private IEnumerator<float> StepTwo()
    {
        yield return Timing.WaitUntilDone(IESetText("Tap until \"HYDROs\" touch the wall"));
        for (int i = 1; i < hydroObjects.Length; i++)
        {
            hydroObjects[i].DOFade(0.3f, 0.1f);
        }

        yield return Timing.WaitUntilTrue(() => _nextTap);
        _nextTap = false;
        //Vector3 startPos = hydroObjects[0].rectTransform.localPosition;
        inEffect.DOFade(1f, 0f);
        inEffect.DOFade(0.3f, 1f).SetLoops(-1, LoopType.Restart);
        inEffect.transform.DOScale(2.4f, 1f).SetLoops(-1, LoopType.Restart);
        handRectTransform.gameObject.SetActive(true);
        /*handRectTransform.DOLocalMoveY(-100, 0.5f).SetEase(Ease.InSine).SetLoops(-1, LoopType.Yoyo)
            .SetRelative(true);*/
        lineTutorial.gameObject.SetActive(true);
        lineTutorial.fillAmount = 1f;
        lineTutorial.DOFillAmount(0.7f, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            hydroObjects[0].DOFade(1, 0.1f);

            DoPathHydro(1);
            hydroObjects[1].DOFade(1, 0.5f);
        });

        yield return Timing.WaitForSeconds(0.5f);
        yield return Timing.WaitUntilTrue(() => _nextTap);
        _nextTap = false;
        lineTutorial.DOFillAmount(0.35f, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            hydroObjects[1].DOFade(0, 0.1f);
            DoPathHydro(2);
            hydroObjects[2].DOFade(1, 0.5f);
        });

        yield return Timing.WaitForSeconds(0.5f);
        yield return Timing.WaitUntilTrue(() => _nextTap);
        _nextTap = false;

        lineTutorial.DOFillAmount(0.09f, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            hydroObjects[2].DOFade(0, 0.1f);
            DoPathHydro(3);
            hydroObjects[3].DOFade(1, 0.5f);
        });

        yield return Timing.WaitForSeconds(0.5f);
        yield return Timing.WaitUntilTrue(() => _nextTap);
        _nextTap = false;

        effectTouchWall.DOFade(1, 0.5f);
        hydroObjects[3].DOFade(0, 0.5f);

        yield return Timing.WaitUntilTrue(() => _nextTap);
        _nextTap = false;

        lineTutorial.DOFillAmount(0, 0.5f).SetEase(Ease.Linear);
        yield return Timing.WaitForSeconds(1f);
        inEffect.gameObject.SetActive(false);
        handRectTransform.gameObject.SetActive(false);
        //hydroObjects[0].rectTransform.localPosition = startPos;
        _isCanNextStep = true;
    }

    private IEnumerator<float> StepThree()
    {
        effectTouchWall.DOFade(0, 0.2f);
        pointGreenImage.DOFade(1, 0.2f);
        pointText.SetText("0");
        yield return Timing.WaitForSeconds(0.2f);
        pointArrowRectTransform.gameObject.SetActive(true);
        pointGreenImage.DOFade(0, 0.5f).SetEase(Ease.InSine).SetLoops(-1, LoopType.Yoyo);
        pointGreenImage.rectTransform.DOScale(1.5f, 0.5f).SetEase(Ease.InSine).SetLoops(-1, LoopType.Yoyo);
        pointArrowRectTransform.DOLocalMoveY(-10, 0.5f).SetEase(Ease.InSine).SetLoops(-1, LoopType.Yoyo)
            .SetRelative(true);
        yield return Timing.WaitUntilDone(IESetText("When \"HYDROs\" touch the wall to get one point"));
        yield return Timing.WaitForSeconds(0.2f);
        pointText.SetText("1");
        yield return Timing.WaitForSeconds(1f);
        _isCanNextStep = true;
    }

    private IEnumerator<float> StepFour()
    {
        hydroObjects[0].DOFade(0, 0.1f);
        yield return Timing.WaitForSeconds(0.1f);
        thornRectTransform.DOLocalMoveX(-100, 0.5f).SetEase(Ease.InBack).SetRelative(true);
        yield return Timing.WaitForSeconds(0.2f);
        yield return Timing.WaitUntilDone(IESetText("If \"HYDROs\" touch the thorn, they will die"));
        yield return Timing.WaitForSeconds(1f);
        hydroDeadImage.DOFade(1, 0.5f);
        yield return Timing.WaitForSeconds(0.5f);
        deadEffectImage.DOFade(1, 0.5f);
        yield return Timing.WaitForSeconds(1f);
        _isCanNextStep = true;
    }

    private IEnumerator<float> StepFive()
    {
        yield return Timing.WaitUntilDone(IESetText("Get ready to play"));
        yield return Timing.WaitForSeconds(1f);
        frameText.SetActive(false);
        mainContent.SetActive(false);
        getReadyImage.DOFade(1, 0.5f);
        yield return Timing.WaitForSeconds(0.5f);
        //yield return Timing.WaitForSeconds(1f);
        SetComplete();
    }

    private IEnumerator<float> IESetText(string text)
    {
        DOTweenTMPAnimator animator = new DOTweenTMPAnimator(hydroText);
        hydroText.SetText(text);
        hydroText.color = new Color(hydroText.color.r, hydroText.color.g, hydroText.color.b, 0f);
        yield return Timing.WaitForSeconds(0.01f);
        for (int i = 0; i < animator.textInfo.characterCount; ++i)
        {
            if (!animator.textInfo.characterInfo[i].isVisible) continue;
            animator.DOFadeChar(i, 1f, 0.01f);
            yield return Timing.WaitForSeconds(0.01f);
        }
    }

    private void DoPathHydro(int toIndex)
    {
        hydroObjects[0].rectTransform
            .DOLocalJump(hydroObjects[toIndex].rectTransform.localPosition, jumpPower, 1, duration)
            .SetEase(ease);
    }

    public override void Hide()
    {
        base.Hide();
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        // kill all do tween
        DOTween.KillAll();
    }

    public void SetComplete()
    {
        _isComplete = true;
        LoadingManager.Instance.Transition(TransitionType.Fade, getReadyImage, () =>
        {
            {
                _completeCallback?.Invoke();
                _completeCallback = null;
            }
        }, Hide);
    }
}