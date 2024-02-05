using FreakyBall.Abilities;

public class FrogEgg : BaseEntity
{
    public override void StartGame()
    {
        InGamePanel inGamePanel = UIManager.Instance.GetScreen<InGamePanel>();
        AbilitySystem abilitySystem = GetComponent<AbilitySystem>();
        abilitySystem.Initialize(inGamePanel, new string[] { "Soul Ability" });
        
        base.StartGame();
    }
    
    public override void OnPlayerCommand(PlayerCommandData playerCommandData)
    {
        base.OnPlayerCommand(playerCommandData);
    }
}