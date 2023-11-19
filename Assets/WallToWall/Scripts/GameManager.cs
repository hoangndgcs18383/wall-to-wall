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

    private InGamePanel inGamePanel;
    private bool _isMoreThanBestScore;

    void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;
        Time.timeScale = 1.0f;
        //background.gameObject.SetActive(true);
    }


    private void Start()
    {
        //InGameManager.Instance.UpdateBestScore(PlayerPrefs.GetInt("BestScore", 0).ToString());
        // player.sprite = MainMenuManager.Instance.GetCurrentSkin();
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

        //SkinManager.Instance.AddListenerSkinColorChanged(SkinColorChanged);
        //SkinManager.Instance.Initialize();

        inGamePanel = UIManager.Instance.GetScreen<InGamePanel>();

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
        inGamePanel.UpdateScore(score.ToString());

        if (score > PlayerPrefs.GetInt("BestScore", 0))
        {
            PlayerPrefs.SetInt("BestScore", score);
            _isMoreThanBestScore = true;
        }

        //SkinManager.Instance.SetSkinSprite(score);
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
        inGamePanel.ShowGameOverEffect();
        Time.timeScale = 0.1f;
        yield return Player.IEDeadAnimation();
        yield return new WaitForSecondsRealtime(0.5f);
        RankManager.Instance.SetRank(score);
        UIManager.Instance.ShowGameOverScreen(new TotalScoreUIData(score,
            PlayerPrefs.GetInt("BestScore", 0), _isMoreThanBestScore));
        inGamePanel.HideGameOverEffect();
        Time.timeScale = 1f;
        inGamePanel.UpdateScore("0");
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    public void Restart()
    {
        //RankManager.Instance.SetRank(score);
        isStarted = false;

        AudioManager.Instance.PlayBGM("BGM_INGAME", volume: 0.3f);
        inGamePanel.Show();
        SceneManager.LoadSceneAsync("InGame");
        RankManager.Instance.RemoveListenerRankChanged(RankChanged);
    }

    private void OnDestroy()
    {
        PoolManager.Instance.ClearPool();
    }
}