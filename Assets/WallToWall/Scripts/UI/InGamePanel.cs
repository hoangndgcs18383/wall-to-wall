using TMPro;
using UnityEngine;

public class InGamePanel : BaseScreen
{
    [SerializeField] private TMP_Text currentScoreText;
    [SerializeField] private GameObject gameOverEffectPanel;
    [SerializeField] private ButtonW2W tapToStart;
    [SerializeField] private ButtonW2W btnPause;
    [SerializeField] private GameObject tapToPlay;

    public override void Initialize()
    {
        base.Initialize();
        tapToStart.onClick.AddListener(StartGame);
        btnPause.onClick.AddListener(OnPauseGame);
    }

    public override void Show(IUIData data = null)
    {
        base.Show(data);
        tapToStart.gameObject.SetActive(true);
        tapToPlay.SetActive(true);
    }

    public void ShowGameOverEffect()
    {
        gameOverEffectPanel.SetActive(true);
    }

    public void HideGameOverEffect()
    {
        gameOverEffectPanel.SetActive(false);
    }

    public void UpdateScore(string score)
    {
        currentScoreText.SetText(score);
    }
    
    public void StartGame()
    {
        GameManager.Instance.GameStart();
        btnPause.gameObject.SetActive(true);
        tapToStart.gameObject.SetActive(false);
        tapToPlay.SetActive(false);
        AudioManager.Instance.PlayBGM("BGM_INGAME", volume: 0.3f);
    }

    public void OnPauseGame()
    {
        UIManager.Instance.ShowPauseScreen();
    }
}