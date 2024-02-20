using System;
using Hzeff.Events;
using TMPro;
using UnityEngine;

namespace FreakyBall.Abilities
{
    public struct AbilityStateData : IEvent
    {
        public string abilityName;
        public PlayerState playerState;
        public string message;
    }

    public class AbilitySystem : MonoBehaviour
    {
        [SerializeField] private AbilityData[] startingAbilities;
        [SerializeField] private TMP_Text stateText;

        private AbilityView _abilityView;
        private AbilityController _abilityController;
        private EventBinding<AbilityStateData> _abilityStateData;

        public void Initialize(InGamePanel inGamePanel, params string[] abilityExcepts)
        {
            if (abilityExcepts.Length > 0)
                // TODO: Check if this is correct => removed the ! from the condition
                startingAbilities = Array.FindAll(startingAbilities,
                    ability => !Array.Exists(abilityExcepts, except => ability.name == except));

            Debug.Log("Starting abilities: " + startingAbilities.Length);

            _abilityView = inGamePanel.AbilityView;
            //_abilityView.gameObject.SetActive(true);
            _abilityController = new AbilityController.Builder().WithAbilities(startingAbilities).Build(_abilityView);

            _abilityStateData = new EventBinding<AbilityStateData>(OnPlayerChangeState);
            EventDispatcher<AbilityStateData>.Register(_abilityStateData);
        }

        public void Dispose()
        {
            EventDispatcher<AbilityStateData>.Unregister(_abilityStateData);
        }

        public void OnPlayerChangeState(AbilityStateData abilityName)
        {
#if UNITY_EDITOR
            switch (abilityName.playerState)
            {
                case PlayerState.Idle:
                case PlayerState.Input:
                    stateText.SetText(abilityName.message);
                    break;
                case PlayerState.Ability:
                    stateText.SetText($"Using {abilityName.abilityName}");
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