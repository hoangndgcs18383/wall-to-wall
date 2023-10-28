using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [HideInInspector] public int score = 0;

    public TMP_Text RankText;

    public SpriteRenderer player;
    
    public TextMeshProUGUI CurrentScoreText;
    public TextMeshProUGUI BestScoreText;
    public TextMeshProUGUI BestText;
    public GameObject TouchToStartText;

    public GameObject GameOverPanel;
    public GameObject GameOverEffectPanel;

    [HideInInspector] public bool isStarted = false;


    static int PlayCount;


    void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;

        Time.timeScale = 1.0f;
        BestScoreText.text = PlayerPrefs.GetInt("BestScore", 0).ToString();
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0) && isStarted == false)
        {
            gameStart();
        }
    }

    [Button]
    public void CheatRank()
    {
        RankManager.Instance.CheatRank();
    }

    private void gameStart()
    {
        isStarted = true;
        TouchToStartText.SetActive(false);
        RankManager.Instance.AddListenerRankChanged(RankChanged);
        RankManager.Instance.Initialize();
        
        SkinManager.Instance.AddListenerSkinColorChanged(SkinColorChanged);
        SkinManager.Instance.Initialize();
    }

    private void RankChanged()
    {
        StringBuilder sb = new StringBuilder();

        foreach (var rank in RankManager.Instance.GetRankList)
        {
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
        CurrentScoreText.text = score.ToString();

        if (score > PlayerPrefs.GetInt("BestScore", 0))
        {
            PlayerPrefs.SetInt("BestScore", score);
            BestScoreText.text = PlayerPrefs.GetInt("BestScore", 0).ToString();
        }

        SkinManager.Instance.SetSkinColor(score);
    }


    public void Gameover()
    {
        StartCoroutine(GameoverCoroutine());
    }


    IEnumerator GameoverCoroutine()
    {
        CurrentScoreText.color = Color.white;
        BestScoreText.color = Color.white;
        BestText.color = Color.white;

        GameOverEffectPanel.SetActive(true);
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(0.5f);
        GameOverPanel.SetActive(true);
        yield break;
    }


    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        RankManager.Instance.RemoveListenerRankChanged(RankChanged);
        SkinManager.Instance.RemoveListenerSkinColorChanged(SkinColorChanged);
    }
}