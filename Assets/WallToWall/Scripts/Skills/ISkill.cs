using System;

public interface ISkill
{
    event Action OnSkillRelease;
    void Initialize(Player player);
    void ReleaseSkill();
    SkillDataConfig GetSkillDataConfig();
}