using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TotalScoreUIData : IUIData
{
    public int CurrentScore;
    public int BestScore;
    public bool IsNewBestScore;

    public TotalScoreUIData(int currentScore, int bestScore, bool isNewBestScore)
    {
        CurrentScore = currentScore;
        BestScore = bestScore;
        IsNewBestScore = isNewBestScore;
    }
}

public class GameOverPanel : BaseScreen
{
    [SerializeField] private ButtonW2W restartButton;
    [SerializeField] private ButtonW2W homeButton;
    [SerializeField] private ButtonW2W shareButton;

    [SerializeField] private TMP_Text bestScoreText;
    [SerializeField] private TMP_Text currentScoreText;

    [SerializeField] private float delayBest = 0.1f;
    [SerializeField] private float delayCurrent = 0.2f;

    [SerializeField] private GameObject newBestScore;
    [SerializeField] private TMP_Text newBestScoreText;

    public override void Initialize()
    {
        base.Initialize();
        restartButton.onClick.AddListener(OnRestart);
        homeButton.onClick.AddListener(OnHome);
        shareButton.onClick.AddListener(OnShare);
    }

    public override void Show(IUIData data = null)
    {
        base.Show(data);
        if (data is TotalScoreUIData totalScoreUIData)
        {
            if (totalScoreUIData.IsNewBestScore)
            {
                newBestScore.SetActive(true);
                newBestScoreText.SetText(totalScoreUIData.CurrentScore.ToString());
            }
            else
            {
                newBestScore.SetActive(false);
            }

            UpdateTotal(totalScoreUIData.CurrentScore.ToString(), totalScoreUIData.BestScore.ToString());
        }
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
        AudioManager.Instance.PlayBGM("BGM_MENU", volume: 0.3f);
        UIManager.Instance.GetScreen<InGamePanel>().Hide();
        UIManager.Instance.GetScreen<InGamePanel>().EndGame();
        UIManager.Instance.ShowMainMenuScreen();
        Hide();
    }

    private void OnShare()
    {
        //ShareSocialManager.Instance.ShareFacebook();
        StartCoroutine(TakeScreenshotAndShare());
    }

    private IEnumerator TakeScreenshotAndShare()
    {
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
        File.WriteAllBytes(filePath, ss.EncodeToPNG());

        // To avoid memory leaks
        Destroy(ss);

        new NativeShare().AddFile(filePath)
            .SetSubject("Say something to your friend!").SetText("I got " + currentScoreText.text + " points!")
            .SetCallback((result, shareTarget) =>
                Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
            .Share();
    }

    private void OnRestart()
    {
        GameManager.Instance.Restart();
        Hide();
    }
}