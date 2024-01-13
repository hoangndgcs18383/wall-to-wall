using System;
using System.Collections.Generic;
using MEC;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "ScriptableObjects/Skill", order = 1)]
public class SkillSO : ScriptableObject, ISkill
{
    [SerializeField] private SkillDataConfig skillDataConfig;

    private Player _player;
    private Vector2 _playerDirection;
    private float _lastTimeUseSkill;

    public event Action OnSkillRelease;

    public void Initialize(Player player)
    {
        _player = player;
        _lastTimeUseSkill = Time.time;
    }

    private void OnEnable()
    {
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
        _playerDirection = _player.GetDirection();

        if (_playerDirection.x > 0)
        {
            if (skillDataConfig.IsUseEffect && skillDataConfig.Effect)
            {
                GameObject obj = Instantiate(skillDataConfig.Effect, _player.transform.position + new Vector3(1, 0, 0),
                    Quaternion.identity);
                Timing.RunCoroutine(ReturnPool(skillDataConfig.Effect, obj));
            }
        }
        else
        {
            if (skillDataConfig.IsUseEffect && skillDataConfig.Effect)
            {
                GameObject obj = Instantiate(skillDataConfig.Effect, _player.transform.position + new Vector3(-1, 0, 0),
                    Quaternion.identity);
                Timing.RunCoroutine(ReturnPool(skillDataConfig.Effect, obj));
            }
        }
        
        OnSkillRelease?.Invoke();
    }

    IEnumerator<float> ReturnPool(GameObject obj, GameObject objPool)
    {
        yield return Timing.WaitForSeconds(1f);
        objPool.SetActive(false);
    }

    public SkillDataConfig GetSkillDataConfig()
    {
        return skillDataConfig;
    }
}