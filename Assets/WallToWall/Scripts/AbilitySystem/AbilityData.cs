using Sirenix.OdinInspector;
using UnityEngine;

namespace FreakyBall.Abilities
{
    [CreateAssetMenu(fileName = "Ability", menuName = "FreakyBall/AbilityData", order = 0)]
    public class AbilityData : ScriptableObject
    {
        public string abilityName;
        [PreviewField] public Sprite abilityIcon;
        //public GameObject abilityPrefab;
        public float cooldown;
        public float duration;
        public bool isStoppable;
        
        private Vector2 _direction;
        private Vector2 _position;
        
        public virtual void UseAbility()
        {
        }
        
        public virtual void SetCurrentDirection(Vector2 direction, Vector2 position)
        {
            _direction = direction;
            _position = position;
        }
        
        protected virtual Vector3 GetCurrentDirection()
        {
            return _direction;
        }
        
        protected virtual Vector3 GetCurrentPosition()
        {
            return _position;
        }
    }
}