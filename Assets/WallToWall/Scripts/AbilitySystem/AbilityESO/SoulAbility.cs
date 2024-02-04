using System.Collections.Generic;
using FreakyBall.Abilities;
using MEC;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "Abilities/SoulAbility")]
public class SoulAbility : AbilityData
{
    public override void UseAbility()
    {
        Timing.RunCoroutine(IERevival());
    }

    public IEnumerator<float> IERevival()
    {
        Debug.Log("Revival");
        IEntity player = null;
        player = GameManager.Instance.GetPlayer();
        yield return Timing.WaitForOneFrame;
        player.GetMaterial().EnableKeyword(ShaderKeys.GHOST_ON);
        player.SetLayer(LayerMask.NameToLayer(TagsKeys.GHOST));
        player.ResetPlayerDefault();
        player.SetPlayerPosition(Vector3.one);
        GameManager.Instance.IsStarted = false;

        yield return Timing.WaitForSeconds(duration);

        GameManager.Instance.IsStarted = true;
        player.ContinueGame();
        player.GetMaterial().DisableKeyword(ShaderKeys.GHOST_ON);
        player.SetLayer(LayerMask.NameToLayer(TagsKeys.PLAYER));
    }
}