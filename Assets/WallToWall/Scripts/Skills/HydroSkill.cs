using System;
using System.Collections.Generic;
using MEC;
using UnityEngine;
using Object = UnityEngine.Object;

public class HydroSkill : ISkillRelease
{
    private Player _player;
    private Vector2 _playerDirection;
    private SkillDataConfig skillDataConfig;
    private GameObject _objPool;
    private float _offsetX = 1f;

    public HydroSkill(Player player)
    {
        _player = player;
    }

    public void SetConfig(SkillDataConfig config)
    {
        skillDataConfig = config;
    }

    public void ReleaseSkill()
    {
        _playerDirection = _player.GetDirection();

        if (_playerDirection.x > 0)
        {
            if (skillDataConfig.IsUseEffect && skillDataConfig.Effect)
            {
                if (_objPool == null)
                    _objPool = Object.Instantiate(skillDataConfig.Effect,
                        _player.transform.position + new Vector3(_offsetX, 0, 0),
                        Quaternion.identity);
                else
                {
                    _objPool.transform.position = _player.transform.position + new Vector3(_offsetX, 0, 0);
                }

                _objPool.SetActive(true);
                Timing.RunCoroutine(ReturnPool(skillDataConfig.Effect, _objPool));
            }
        }
        else
        {
            if (skillDataConfig.IsUseEffect && skillDataConfig.Effect)
            {
                if (_objPool == null)
                    _objPool = Object.Instantiate(skillDataConfig.Effect,
                        _player.transform.position + new Vector3(-_offsetX, 0, 0),
                        Quaternion.identity);
                else
                {
                    _objPool.transform.position = _player.transform.position + new Vector3(-_offsetX, 0, 0);
                }

                _objPool.SetActive(true);
                Timing.RunCoroutine(ReturnPool(skillDataConfig.Effect, _objPool));
            }
        }
    }

    IEnumerator<float> ReturnPool(GameObject obj, GameObject objPool)
    {
        yield return Timing.WaitForSeconds(1f);
        objPool.SetActive(false);
    }
}