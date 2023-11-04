using System;
using UnityEngine;
using UnityEngine.UI;

public class InGamePanel : MonoBehaviour
{
    [SerializeField] private GameObject gameOverEffectPanel;
    [SerializeField] private GameObject btnPause;
    [SerializeField] private ButtonW2W tapToStart;

    private void OnEnable()
    {
        tapToStart.onClick.AddListener(StartGame);
    }
    
    private void OnDisable()
    {
        tapToStart.onClick.RemoveListener(StartGame);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        tapToStart.gameObject.SetActive(true);
    }
    
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
    public void ShowGameOverEffect()
    {
        gameOverEffectPanel.SetActive(true);
    }
    
    public void HideGameOverEffect()
    {
        gameOverEffectPanel.SetActive(false);
    }

    public void StartGame()
    {
        GameManager.Instance.GameStart();
        btnPause.SetActive(true);
        tapToStart.gameObject.SetActive(false);
    }
}
