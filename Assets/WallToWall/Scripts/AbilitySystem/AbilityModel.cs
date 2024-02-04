namespace FreakyBall.Abilities
{
    public class AbilityModel
    {
        public readonly ObservableList<Ability> abilities = new();

        public void AddAbility(Ability ability)
        {
            abilities.Add(ability);
        }
    }
}