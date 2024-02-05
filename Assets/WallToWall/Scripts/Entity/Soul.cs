using System;
using FreakyBall.Abilities;

public class Soul : BaseEntity
{
    private Action RevivalAction;

    public override void StartGame()
    {
        base.StartGame();
        InGamePanel inGamePanel = UIManager.Instance.GetScreen<InGamePanel>();
        AbilitySystem.Initialize(inGamePanel);
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
        RevivalAction = playerCommandData.abilityData.PassiveAbility;
    }
}