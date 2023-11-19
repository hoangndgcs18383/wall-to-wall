using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FlashScreen : MonoBehaviour
{
    [SerializeField] private Image logoTf;
    [SerializeField] private ButtonW2W touchToStartButton;
    [SerializeField] private Image background;

    private void Start()
    {
        logoTf.rectTransform.DOAnchorPosY(50f, 2f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        touchToStartButton.onClick.AddListener(OnClickStart);
    }

    private void OnClickStart()
    {
        LoadingManager.Instance.Transition(TransitionType.Fade, background, () =>
        {
            StartOrEndTransition(true);
            AudioManager.Instance.StopBGM();
            UIManager.Instance.ShowMainMenuScreen();
        }, () =>
        {
            StartOrEndTransition(false);
            AudioManager.Instance.PlayBGM("BGM_MENU", volume: 0.3f);
        });
    }
    
    private void StartOrEndTransition(bool isStart)
    {
        gameObject.SetActive(isStart);
        touchToStartButton.gameObject.SetActive(!isStart);
        logoTf.gameObject.SetActive(!isStart);
    }

    private void OnDestroy()
    {
        touchToStartButton.onClick.RemoveAllListeners();
    }
}