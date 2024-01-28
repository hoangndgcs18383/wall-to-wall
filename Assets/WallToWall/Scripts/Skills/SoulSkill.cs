using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulSkill : ISkillRelease
{
    private SkillDataConfig skillDataConfig;
    private Player _player;
    private bool _hasTriggerPassiveSkill;

    public SoulSkill(Player player)
    {
        _player = player;
    }

    public void SetConfig(SkillDataConfig config)
    {
        skillDataConfig = config;
        _hasTriggerPassiveSkill = false;
        _player.CanBeContinue(true);
    }

    public void ReleaseSkill()
    {
        if (!_hasTriggerPassiveSkill)
            _hasTriggerPassiveSkill = true;
        
        _player.CanBeContinue(false);
        RandomFlexiblePosition();
        _player.SetConfigScaleGravity();
    }
    
    private void RandomFlexiblePosition()
    {
        _player.GetRigidbody2D().velocity = new Vector2(0, 0);
        _player.GetRigidbody2D().gravityScale = 0;
        var randomPosition = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
        _player.transform.position = randomPosition;
    }
}