using System;
using System.Collections;
using System.Collections.Generic;
using FreakyBall.Abilities;
using MEC;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "Abilities/SnowAbility")]
public class SnowAbility : AbilityData
{
    [SerializeField] private GameObject snowDetector;

    public override void UseAbility()
    {
        base.UseAbility();
        PoolManager.Instance.CreateOrGetPool(snowDetector.gameObject, 2, o =>
        {
            IEntity player = GameManager.Instance.GetPlayer();
            player.SaveCurrentVelocity();
            player.StopImmediate();
            o.transform.position = GetCurrentPosition();
            o.SetActive(true);
            SnowDetector snow = o.GetComponent<SnowDetector>();
            snow.StartAnimation();
            Timing.RunCoroutine(ReturnPool(o, gameObject =>
            {
                player.ContinueGame();
                player.RestoreVelocity();
                CompleteAbility();
            }));
        });
    }

    public IEnumerator<float> ReturnPool(GameObject obj, Action<GameObject> onActionComplete)
    {
        yield return Timing.WaitForSeconds(duration);
        PoolManager.Instance.ReturnPool(snowDetector.gameObject, obj, true, onActionComplete);
    }
}