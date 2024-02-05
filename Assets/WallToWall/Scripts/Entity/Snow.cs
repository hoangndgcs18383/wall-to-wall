public class Snow : BaseEntity
{
    public override void StartGame()
    {
        InGamePanel inGamePanel = UIManager.Instance.GetScreen<InGamePanel>();
        AbilitySystem.Initialize(inGamePanel, new string[] { "Soul Ability" });
        
        base.StartGame();
    }
    
}
