using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface IUIData
{
}

public class TotalScoreUIData : IUIData
{
    public string CurrentScore;
    public string BestScore;

    public TotalScoreUIData(string currentScore, string bestScore)
    {
        CurrentScore = currentScore;
        BestScore = bestScore;
    }
}

public class GameOverPanel : MonoBehaviour
{
    [SerializeField] private ButtonW2W restartButton;
    [SerializeField] private ButtonW2W homeButton;

    [SerializeField] private TMP_Text bestScoreText;
    [SerializeField] private TMP_Text currentScoreText;

    [SerializeField] private float delayBest = 0.1f;
    [SerializeField] private float delayCurrent = 0.2f;

    public void Show()
    {
        gameObject.SetActive(true);

        restartButton.onClick.AddListener(OnRestart);
        homeButton.onClick.AddListener(OnHome);
    }

    public void SetData(IUIData data)
    {
        TotalScoreUIData _data = data as TotalScoreUIData;
        UpdateTotal(_data.CurrentScore, _data.BestScore);
    }

    public void Hide()
    {
        gameObject.SetActive(false);

        restartButton.onClick.RemoveListener(OnRestart);
        homeButton.onClick.RemoveListener(OnHome);
    }

    public void UpdateTotal(string current, string best)
    {
        currentScoreText.SetText(string.Empty);
        bestScoreText.SetText(string.Empty);
        currentScoreText.DOText(current, delayCurrent, true, ScrambleMode.Numerals);
        bestScoreText.DOText(best, delayBest, true, ScrambleMode.Numerals);
    }

    private void OnHome()
    {
        SceneManager.LoadSceneAsync("Menu");
        MainMenuManager.Instance.Show();
        AudioManager.Instance.PlayBGM("BGM_MENU", volume: 0.3f);
        InGameManager.Instance.ShowOrHideInGamePanel(false);
        Hide();
    }

    private void OnRestart()
    {
        GameManager.Instance.Restart();
        Hide();
    }
}