using System.Collections.Generic;
using FreakyBall.Abilities;
using MEC;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "Abilities/ShinyAbility")]
public class ShinyAbility : AbilityData
{
    public override void UseAbility()
    {
        Timing.RunCoroutine(IEStopAndChooseDirection());
    }

    public IEnumerator<float> IEStopAndChooseDirection()
    {
        IEntity player = null;
        player = GameManager.Instance.GetPlayer();
        yield return Timing.WaitForOneFrame;
        player.StopImmediate();
        player.SetLineRendererToChooseDirection();
    }
}