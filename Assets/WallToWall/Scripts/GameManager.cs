﻿using System;
using System.Collections;
using System.Text;
using FreakyBall.Abilities;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private PlayerConfig playerConfig;

    [HideInInspector] public int score = 0;

    public TMP_Text RankText;

    //public SpriteRenderer player;

    [HideInInspector] public bool IsStarted = false;
    static int PlayCount;

    //public Player Player;

    private IEntity _player;

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
        //player.sprite = SkinManager.Instance.GetCurrentSkin().unlockSprite;

        
        inGamePanel = UIManager.Instance.GetScreen<InGamePanel>();
        GameObject player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

        switch (SkinManager.Instance.GetCurrentSkin().hash)
        {
            case "hyro_skin":
            case "shiny_skin":
                _player = player.AddComponent<Hydro>();
                break;
            case "soul_skin":
                _player = player.AddComponent<Soul>();
                break;
            case "snow_skin":
                _player = player.AddComponent<Snow>();
                break;
            case "eggfrog_skin":
                _player = player.AddComponent<FrogEgg>();
                break;
            default:
                _player = player.AddComponent<BaseEntity>();
                break;
        }

        _player?.Initialize(playerConfig, SkinManager.Instance.GetCurrentSkin());
        _player?.SetSkin(SkinManager.Instance.GetCurrentSkin().unlockSprite);
    }

    [Button]
    public void CheatRank()
    {
        RankManager.Instance.CheatRank();
    }

    public void GameStart()
    {
        if (IsStarted) return;
        IsStarted = true;
        _player?.StartGame();
        //Player.StartPlayer();

        RankManager.Instance.AddListenerRankChanged(RankChanged);
        //SkinManager.Instance.AddListenerSkinColorChanged(SkinColorChanged);
        //SkinManager.Instance.Initialize();
        //score = Player.playerConfig.startScore;
        inGamePanel = UIManager.Instance.GetScreen<InGamePanel>();
        inGamePanel.UpdateScore(score.ToString());


        TriangleManager.Instance.StartGame();

        //background.gameObject.SetActive(false);
        //InGameManager.Instance.InGamePanelStart();
    }

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

    public void AddScore()
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


    public void GameOver()
    {
        StartCoroutine(GameoverCoroutine());
    }


    IEnumerator GameoverCoroutine()
    {
        /*CurrentScoreText.color = Color.white;
        BestScoreText.color = Color.white;
        BestText.color = Color.white;*/
        inGamePanel.ShowGameOverEffect();
        yield return new WaitForSecondsRealtime(0.1f);
        //Time.timeScale = 0.1f;
        //yield return Player.IEDeadAnimation();
        _player?.AddDeathCount();
        _player?.SetActiveSprite(false);
        _player?.PlayDeadAnimation();
        //yield return new WaitUntil(() => !Player.IsDeadAnimationPlaying());
        yield return new WaitForSecondsRealtime(1.5f);
        RankManager.Instance.SetRank(score);
        UIManager.Instance.ShowGameOverScreen(new TotalScoreUIData(score,
            PlayerPrefs.GetInt("BestScore", 0), _isMoreThanBestScore));
        inGamePanel.HideGameOverEffect();
        //Time.timeScale = 1f;
        _player?.DisableAnimator();
        _player?.Dispose();

        if (SaveSystem.Instance.GetInt(PrefKeys.DeathCount) >= GameConstant.AdsTriggerCount)
        {
            SaveSystem.Instance.SetInt(PrefKeys.DeathCount, 0);
#if UNITY_ANDROID || UNITY_IOS
            AdsManager.Instance.LoadInterstitial();
#endif
        }
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
        IsStarted = false;

        AudioManager.Instance.PlayBGM("BGM_INGAME", volume: 0.3f);
        inGamePanel.Show();
        inGamePanel.ResetStartGame();
        SceneManager.LoadSceneAsync("InGame");
        RankManager.Instance.RemoveListenerRankChanged(RankChanged);
    }

    public IEntity GetPlayer()
    {
        return _player;
    }

    private void OnDestroy()
    {
        PoolManager.Instance.ClearPool();
    }
}