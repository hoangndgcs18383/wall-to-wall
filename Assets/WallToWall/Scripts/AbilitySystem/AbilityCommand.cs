using Hzeff.Events;
using UnityEngine.Rendering;

namespace FreakyBall.Abilities
{
    public class AbilityCommand : ICommand
    {
        private readonly AbilityData _abilityData;
        public float countdown;
        
        public AbilityCommand(AbilityData abilityData)
        {
            _abilityData = abilityData;
            countdown = abilityData.cooldown;
        }
        
        public void Execute()
        {
            //Do something
            EventDispatcher<PlayerCommandData>.Dispatch(new PlayerCommandData
            {
                abilityData = _abilityData
            });
        }
    }

    public struct PlayerCommandData : IEvent
    {
        public AbilityData abilityData;
    }
}