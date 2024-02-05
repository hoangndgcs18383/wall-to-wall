using FreakyBall.Abilities;

public class Hydro : BaseEntity
{
    public override void StartGame()
    {
        base.StartGame();
        InGamePanel inGamePanel = UIManager.Instance.GetScreen<InGamePanel>();
        AbilitySystem.Initialize(inGamePanel, new string[] { "Soul Ability" });
    }

    public override void OnPlayerCommand(PlayerCommandData playerCommandData)
    {
        base.OnPlayerCommand(playerCommandData);
    }
}