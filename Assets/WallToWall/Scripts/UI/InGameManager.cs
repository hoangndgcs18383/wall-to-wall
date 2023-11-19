using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class InGameManager : Singleton<InGameManager>
{
    //[SerializeField] private CanvasGroup canvasGroup;
    
    #region References
    
    //[SerializeField] private InGamePanel inGamePanel;
    [SerializeField] private GameOverPanel gameOverPanel;

    #endregion

    [BoxGroup("Texts")] 
    [SerializeField] private TMP_Text bestScoreText;
    [SerializeField] private TMP_Text currentScoreText;

    public void Show()
    {
        /*canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;*/
    }
    
    public void Hide()
    {
        /*canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;*/
    }

    private void Start()
    {
        Hide();
    }

    #region InGamePanel

    public void UpdateBestScore(string value)
    {
        bestScoreText.SetText(value);
    }
    
    public void UpdateCurrentScore(string value)
    {
        currentScoreText.SetText(value);
    }

    public void Reload()
    {
        //inGamePanel.Show();
        AudioManager.Instance.PlayBGM("BGM_INGAME", volume: 0.3f);
        Show();
    }
    
    public void ShowOrHideInGamePanelEffect(bool e)
    {
        /*if(e) inGamePanel.ShowGameOverEffect();
        else inGamePanel.HideGameOverEffect();*/
    }
    
    public void ShowOrHideInGamePanel(bool e)
    {
        /*if(e) inGamePanel.Show();
        else inGamePanel.Hide();*/
    }

    #endregion
    
    #region GameOverPanel
    
    public void GameOverPanelShow()
    {
        //gameOverPanel.Show();
        AudioManager.Instance.StopBGM();
    }
    
    public void GameOverPanelSetData(TotalScoreUIData data)
    {
        //gameOverPanel.SetData(data);
    }
    
    public void GameOverPanelHide()
    {
        gameOverPanel.Hide();
    }
    
    #endregion
    
}
