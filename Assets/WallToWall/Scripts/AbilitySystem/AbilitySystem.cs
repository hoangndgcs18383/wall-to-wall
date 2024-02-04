using System;
using System.Collections.Generic;
using MEC;
using TMPro;
using UnityEngine;

namespace FreakyBall.Abilities
{
    public class AbilitySystem : MonoBehaviour
    {
        [SerializeField] private AbilityData[] startingAbilities;
        [SerializeField] private TMP_Text stateText;

        private AbilityView _abilityView;
        private AbilityController _abilityController;

        public void Initialize(InGamePanel inGamePanel, params string[] abilityExcepts)
        {
            if (abilityExcepts.Length > 0)
                // TODO: Check if this is correct => removed the ! from the condition
                startingAbilities = Array.FindAll(startingAbilities,
                    ability => !Array.Exists(abilityExcepts, except => ability.name == except));

            Debug.Log("Starting abilities: " + startingAbilities.Length);

            _abilityView = inGamePanel.AbilityView;
            _abilityView.gameObject.SetActive(true);
            _abilityController = new AbilityController.Builder().WithAbilities(startingAbilities).Build(_abilityView);
            gameObject.SetActive(true);
        }

        public void OnPlayerChangeState(PlayerState state, string abilityName = "")
        {
#if UNITY_EDITOR
            switch (state)
            {
                case PlayerState.Idle:
                case PlayerState.Input:
                    stateText.SetText("");
                    break;
                case PlayerState.Ability:
                    stateText.SetText($"Using {abilityName}");
                    break;
                case PlayerState.ChooseDirection:
                    stateText.SetText("Choose Direction");
                    break;
            }
#endif
        }

        private void Update()
        {
            _abilityController?.Update(Time.deltaTime);
        }
    }
}