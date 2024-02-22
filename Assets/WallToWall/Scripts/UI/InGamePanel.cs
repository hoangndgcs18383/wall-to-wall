using System;
using System.Collections.Generic;
using DG.Tweening;
using FreakyBall.Abilities;
using MEC;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InGamePanel : BaseScreen
{
    [SerializeField] private TMP_Text currentScoreText;
    [SerializeField] private GameObject gameOverEffectPanel;
    [SerializeField] private ButtonW2W tapToStart;
    [SerializeField] private ButtonW2W btnPause;
    [SerializeField] private ButtonW2W btnUseSkill;
    [SerializeField] private Image countDownImage;
    [SerializeField] private GameObject tapToPlay;
    [SerializeField] private GameObject pointFrame;
    [SerializeField] private TMP_Text skillInfoText;
    [SerializeField] private AbilityView abilityView;

    [SerializeField] private bool _isStartGame = false;

    private ISkill _currentSkill;

    public AbilityView AbilityView
    {
        get => abilityView;
        set => abilityView = value;
    }

    public override void Initialize()
    {
        base.Initialize();
        tapToStart.onClick.AddListener(StartGame);
        btnPause.onClick.AddListener(OnPauseGame);
        btnUseSkill.onClick.AddListener(UseSkill);
        btnUseSkill.gameObject.SetActive(false);
        skillInfoText.gameObject.SetActive(false);
#if UNITY_EDITOR
        //EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
        
        AbilityView.gameObject.SetActive(false);
    }

    public override void Show(IUIData data = null)
    {
        base.Show(data);
        tapToStart.gameObject.SetActive(true);
        tapToPlay.SetActive(true);
        pointFrame.SetActive(false);
        _currentSkill = SkillManager.Instance.GetCurrentSkill();
        if (_currentSkill != null) _currentSkill.OnSkillRelease += OnSkillRelease;
    }


    public override void Hide()
    {
        base.Hide();
        if (_currentSkill != null) _currentSkill.OnSkillRelease -= OnSkillRelease;
    }

    public void ShowGameOverEffect()
    {
        gameOverEffectPanel.SetActive(true);
        AbilityView.gameObject.SetActive(false);
    }

    public void EndGame()
    {
        _isStartGame = false;
    }

    public void HideGameOverEffect()
    {
        gameOverEffectPanel.SetActive(false);
        btnPause.gameObject.SetActive(false);
    }

    public void UpdateScore(string score)
    {
        currentScoreText.SetText(score);
    }

    public void StartGame()
    {
        if (_isStartGame) return;
        _isStartGame = true;
        
        GameManager.Instance.GameStart();
        //btnPause.gameObject.SetActive(true);
        pointFrame.SetActive(true);
        tapToStart.gameObject.SetActive(true);
        tapToPlay.SetActive(false);
        AudioManager.Instance.PlayBGM("BGM_INGAME", volume: 0.3f);
    }

    private void OnSkillRelease()
    {
        Timing.RunCoroutine(IECountDown());
    }

    private float _countDown = 3;

    private IEnumerator<float> IECountDown()
    {
        _countDown = _currentSkill.GetSkillDataConfig().CoolDown;
        while (skillInfoText != null && _countDown > 0)
        {
            skillInfoText.SetText($"{Mathf.RoundToInt(_countDown)} to use skill");
            _countDown -= 1f;
            yield return Timing.WaitForSeconds(1f);
        }

        skillInfoText.SetText("Skill ready");
    }

    public void OnPauseGame()
    {
        UIManager.Instance.ShowPauseScreen();
    }

    private void UseSkill()
    {
        if (_currentSkill != null)
        {
            _currentSkill.ReleaseSkill();
            countDownImage.fillAmount = 0;
            countDownImage.DOFillAmount(1, _currentSkill.GetSkillDataConfig().CoolDown).SetEase(Ease.Linear).OnComplete(
                () => { countDownImage.raycastTarget = true; });
            countDownImage.raycastTarget = false;
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && _isStartGame)
        {
            OnPauseGame();
        }
    }

#if UNITY_EDITOR
    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        return;
        if (state == PlayModeStateChange.ExitingPlayMode && _isStartGame)
        {
            OnPauseGame();
        }
    }
#endif
}