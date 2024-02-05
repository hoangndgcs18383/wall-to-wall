using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FreakyBall.Abilities
{
    public class AbilityView : MonoBehaviour
    {
        [SerializeField] public AbilityButton[] abilityButtons;

        private void Awake()
        {
            for (int i = 0; i < abilityButtons.Length; i++)
            {
                abilityButtons[i].Initialize(i);
            }
        }

        [Button]
        public void GetAbilityButtons()
        {
            List<AbilityButton> buttons = new List<AbilityButton>();

            foreach (AbilityButton button in transform.GetComponentsInChildren<AbilityButton>())
            {
                buttons.Add(button);
            }

            abilityButtons = buttons.ToArray();
        }

        public void UpdateProgress(float progress)
        {
            if (float.IsNaN(progress))
            {
                progress = 0;
            }

            for (int i = 0; i < abilityButtons.Length; i++)
            {
                abilityButtons[i].UpdateAbilityProgress(progress);
            }
        }

        public void UpdateSprites(IList<Ability> abilities)
        {
            for (int i = 0; i < abilityButtons.Length; i++)
            {
                if (i < abilities.Count)
                {
                    abilityButtons[i].UpdateAbilitySprite(abilities[i].data.abilityIcon);
                    abilityButtons[i].gameObject.SetActive(true);
                }
                else
                {
                    abilityButtons[i].gameObject.SetActive(false);
                }
            }
        }
    }
}