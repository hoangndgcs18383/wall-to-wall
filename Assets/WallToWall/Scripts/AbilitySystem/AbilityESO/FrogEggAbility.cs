using System.Collections.Generic;
using FreakyBall.Abilities;
using MEC;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityData", menuName = "Abilities/FrogEggAbility")]
public class FrogEggAbility : AbilityData
{
    private static readonly int GhostTransparency = Shader.PropertyToID("_GhostTransparency");
    private static readonly int GhostBlend = Shader.PropertyToID("_GhostBlend");

    //private LocalKeyword _localKeyword;

    public override void UseAbility()
    {
        base.UseAbility();
        Timing.RunCoroutine(IEAnimationInvisible());
    }


    IEnumerator<float> IEAnimationInvisible()
    {
        IEntity player = null;
        player = GameManager.Instance.GetPlayer();
        //_localKeyword = new LocalKeyword(player.GetMaterial().shader, ShaderKeys.GHOST_ON);
        yield return Timing.WaitForOneFrame;
        player.GetMaterial().SetFloat(GhostTransparency, 0.2f);
        player.GetMaterial().SetFloat(GhostBlend, 1f);
        player.SetLayer(LayerMask.NameToLayer(TagsKeys.GHOST));
        yield return Timing.WaitForSeconds(duration);
        player.GetMaterial().SetFloat(GhostTransparency, 0f);
        player.GetMaterial().SetFloat(GhostBlend, 0f);
        player.SetLayer(LayerMask.NameToLayer(TagsKeys.PLAYER));
        CompleteAbility();
    }
}