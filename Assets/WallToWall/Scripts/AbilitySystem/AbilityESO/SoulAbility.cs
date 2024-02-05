using System.Collections.Generic;
using FreakyBall.Abilities;
using MEC;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "Abilities/SoulAbility")]
public class SoulAbility : AbilityData
{
    private IEntity _player;
    private static readonly int GhostTransparency = Shader.PropertyToID("_GhostTransparency");
    private static readonly int GhostBlend = Shader.PropertyToID("_GhostBlend");

    public override void UseAbility()
    {
        base.UseAbility();
    }

    public override void PassiveAbility()
    {
        Timing.RunCoroutine(IERevival());
    }

    public IEnumerator<float> IERevival()
    {
        _player = GameManager.Instance.GetPlayer();
        yield return Timing.WaitForOneFrame;
        _player.GetMaterial().SetFloat(GhostTransparency, 0.2f);
        _player.GetMaterial().SetFloat(GhostBlend, 1f);
        _player.SetLayer(LayerMask.NameToLayer(TagsKeys.GHOST));
        _player.ResetPlayerDefault();
        _player.SetPlayerPosition(Vector3.one);
        GameManager.Instance.IsStarted = false;

        yield return Timing.WaitForSeconds(duration);

        GameManager.Instance.IsStarted = true;
        _player.ContinueGame();
        _player.GetMaterial().SetFloat(GhostTransparency, 0f);
        _player.GetMaterial().SetFloat(GhostBlend, 0f);
        _player.SetLayer(LayerMask.NameToLayer(TagsKeys.PLAYER));
        CompleteAbility();
    }
}