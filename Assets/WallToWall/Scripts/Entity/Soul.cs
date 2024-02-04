using System;
using FreakyBall.Abilities;

public class Soul : BaseEntity
{
    private Action RevivalAction;

    public override void StartGame()
    {
        InGamePanel inGamePanel = UIManager.Instance.GetScreen<InGamePanel>();
        AbilitySystem abilitySystem = GetComponent<AbilitySystem>();
        abilitySystem.Initialize(inGamePanel);
        
        base.StartGame();
    }
    
    protected override void OnEnterTriangleCollision()
    {
        if (IsReceiveLive)
        {
            IsReceiveLive = false;
            RevivalAction?.Invoke();
            RevivalAction = null;
            return;
        }

        base.OnEnterTriangleCollision();
    }

    public override void OnPlayerCommand(PlayerCommandData playerCommandData)
    {
        base.OnPlayerCommand(playerCommandData);
        IsReceiveLive = true;
        RevivalAction = playerCommandData.abilityData.UseAbility;
    }
}