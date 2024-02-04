using System.Collections.Generic;
using FreakyBall.Abilities;
using MEC;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "Abilities/HydroAbility")]
public class HydroAbility : AbilityData
{
    [SerializeField] private GameObject abilityPrefab;
    private GameObject _abilityPrefab;
    
    public override void UseAbility()
    {
        if (GetCurrentDirection().x > 0)
        {
            if (_abilityPrefab)
            {
                if (_abilityPrefab == null)
                {
                    _abilityPrefab = Instantiate(abilityPrefab);
                }

                _abilityPrefab.transform.localPosition = CurrentOffsetPosition(new Vector3(2, 0.2f, 0));
                _abilityPrefab.gameObject.SetActive(true);
            }
        }
        else
        {
            if (abilityPrefab)
            {
                if (_abilityPrefab == null)
                {
                    _abilityPrefab = Instantiate(abilityPrefab);
                }

                _abilityPrefab.transform.localPosition = CurrentOffsetPosition(new Vector3(-2, 0.2f, 0));
                _abilityPrefab.gameObject.SetActive(true);
            }
        }

        Timing.RunCoroutine(DisableGameObject());
    }

    public Vector3 CurrentOffsetPosition(Vector3 offset)
    {
        return GetCurrentPosition() + offset;
    }

    private IEnumerator<float> DisableGameObject()
    {
        yield return Timing.WaitForSeconds(duration);
        if (_abilityPrefab != null)
        {
            _abilityPrefab.gameObject.SetActive(false);
        }
    }
}