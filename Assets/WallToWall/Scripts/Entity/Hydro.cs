using FreakyBall.Abilities;

public class Hydro : BaseEntity
{
    public override void StartGame()
    {
        InGamePanel inGamePanel = UIManager.Instance.GetScreen<InGamePanel>();
        AbilitySystem = GetComponent<AbilitySystem>();
        AbilitySystem.Initialize(inGamePanel, new string[] { "Soul Ability" });
        
        base.StartGame();
    }

    public override void OnPlayerCommand(PlayerCommandData playerCommandData)
    {
        AbilitySystem.OnPlayerChangeState(PlayerState.Ability, playerCommandData.abilityData.abilityName);
        playerCommandData.abilityData.SetCurrentDirection(GetPosition(), transform.position);
        playerCommandData.abilityData.UseAbility();
    }
}