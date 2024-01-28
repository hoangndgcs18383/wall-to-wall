using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "ScriptableObjects/Skill", order = 1)]
public class SkillSO : ScriptableObject, ISkill
{
    [SerializeField] private SkillDataConfig skillDataConfig;

    private float _lastTimeUseSkill;
    private ISkillRelease _skillRelease;

    public event Action OnSkillRelease;

    public void Initialize(ISkillRelease skillRelease)
    {
        //_player = player;
        _lastTimeUseSkill = Time.time;
        _skillRelease = skillRelease;
        _skillRelease?.SetConfig(skillDataConfig);
    }

    private void OnEnable()
    {
        if (!Application.isPlaying) return; 
        SkillManager.Instance.RegisterSkill(skillDataConfig.NameDisplay, this);
    }

    public void ReleaseSkill()
    {
        if (Time.time - _lastTimeUseSkill < skillDataConfig.CoolDown)
        {
            return;
        }

        Debug.Log("Release skill: " + skillDataConfig.NameDisplay);
        _lastTimeUseSkill = Time.time;
        
        _skillRelease?.ReleaseSkill();
        OnSkillRelease?.Invoke();
    }

    public SkillDataConfig GetSkillDataConfig()
    {
        return skillDataConfig;
    }
}