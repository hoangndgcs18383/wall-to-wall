using System.Collections.Generic;
using MEC;
using UnityEngine;

public class FrogSkill : ISkillRelease
{
    private SkillDataConfig _skillDataConfig;
    private Player _player;

    public FrogSkill(Player player)
    {
        _player = player;
    }
    
    public void SetConfig(SkillDataConfig config)
    {
        _skillDataConfig = config;
    }

    public void ReleaseSkill()
    {
        Timing.RunCoroutine(IEAnimationInvisible());
    }

    IEnumerator<float> IEAnimationInvisible()
    {
        yield return Timing.WaitForOneFrame;
        _player.GetMaterial().EnableKeyword(ShaderKeys.GHOST_ON);
        _player.SetLayer(LayerMask.NameToLayer(TagsKeys.GHOST));
        yield return Timing.WaitForSeconds(_skillDataConfig.Duration);
        _player.GetMaterial().DisableKeyword(ShaderKeys.GHOST_ON);
        _player.SetLayer(LayerMask.NameToLayer(TagsKeys.PLAYER));
    }
}
