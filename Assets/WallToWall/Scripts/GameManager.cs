using System;
using System.Collections;
using System.Text;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [HideInInspector] public int score = 0;

    public TMP_Text RankText;

    public SpriteRenderer player;

    [HideInInspector] public bool isStarted = false;
    static int PlayCount;

    public Player Player;


    void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;
        Time.timeScale = 1.0f;
        //background.gameObject.SetActive(true);
    }


    private void Start()
    {
        InGameManager.Instance.UpdateBestScore(PlayerPrefs.GetInt("BestScore", 0).ToString());
        player.sprite = MainMenuManager.Instance.GetCurrentSkin();
    }

    [Button]
    public void CheatRank()
    {
        RankManager.Instance.CheatRank();
    }

    public void GameStart()
    {
        if (IsStarted) return;
        isStarted = true;
        Player.StartPlayer();

        RankManager.Instance.AddListenerRankChanged(RankChanged);
        RankManager.Instance.Initialize();

        //SkinManager.Instance.AddListenerSkinColorChanged(SkinColorChanged);
        SkinManager.Instance.Initialize();

        TriangleManager.Instance.StartGame();

        //background.gameObject.SetActive(false);
        //InGameManager.Instance.InGamePanelStart();
    }

    private bool IsStarted => isStarted;

    private void RankChanged()
    {
        StringBuilder sb = new StringBuilder();

        foreach (var rank in RankManager.Instance.GetRankList)
        {
            if (rank.Value == -1) continue;
            sb.AppendLine($"{rank.Key} : {rank.Value}");
        }

        RankText.SetText(sb);
    }

    private void SkinColorChanged(Color color)
    {
        player.color = color;
    }


    public void addScore()
    {
        score++;
        InGameManager.Instance.UpdateCurrentScore(score.ToString());

        if (score > PlayerPrefs.GetInt("BestScore", 0))
        {
            PlayerPrefs.SetInt("BestScore", score);
            InGameManager.Instance.UpdateBestScore(PlayerPrefs.GetInt("BestScore", 0).ToString());
        }

        SkinManager.Instance.SetSkinColor(score);
    }


    public void Gameover()
    {
        StartCoroutine(GameoverCoroutine());
    }


    IEnumerator GameoverCoroutine()
    {
        /*CurrentScoreText.color = Color.white;
        BestScoreText.color = Color.white;
        BestText.color = Color.white;*/

        InGameManager.Instance.ShowOrHideInGamePanelEffect(true);
        Time.timeScale = 0.1f;
        yield return Player.IEDeadAnimation();
        yield return new WaitForSecondsRealtime(0.5f);
        InGameManager.Instance.GameOverPanelShow();
        InGameManager.Instance.GameOverPanelSetData(new TotalScoreUIData(score.ToString(),
            PlayerPrefs.GetInt("BestScore", 0).ToString()));
        InGameManager.Instance.ShowOrHideInGamePanelEffect(false);
        Time.timeScale = 1f;
        InGameManager.Instance.UpdateBestScore("0");
        InGameManager.Instance.UpdateCurrentScore("0");
        
    }


    public void Restart()
    {
        RankManager.Instance.SetRank(score);
        isStarted = false;

        InGameManager.Instance.Reload();
        SceneManager.LoadSceneAsync("InGame");
        RankManager.Instance.RemoveListenerRankChanged(RankChanged);
        SkinManager.Instance.RemoveListenerSkinColorChanged(SkinColorChanged);
        InGameManager.Instance.UpdateBestScore("0");
    }

    private void OnDestroy()
    {
        PoolManager.Instance.ClearPool();
    }
}