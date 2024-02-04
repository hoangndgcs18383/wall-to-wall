using System.Collections;
using System.Collections.Generic;
using FreakyBall.Abilities;
using MEC;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityData", menuName = "Abilities/FrogEggAbility")]
public class FrogEggAbility : AbilityData
{
    public override void UseAbility()
    {
        Timing.RunCoroutine(IEAnimationInvisible());
    }


    IEnumerator<float> IEAnimationInvisible()
    {
        IEntity player = null;
        player = GameManager.Instance.GetPlayer();

        yield return Timing.WaitForOneFrame;
        player.GetMaterial().EnableKeyword(ShaderKeys.GHOST_ON);
        player.SetLayer(LayerMask.NameToLayer(TagsKeys.GHOST));
        yield return Timing.WaitForSeconds(duration);
        player.GetMaterial().DisableKeyword(ShaderKeys.GHOST_ON);
        player.SetLayer(LayerMask.NameToLayer(TagsKeys.PLAYER));
    }
}