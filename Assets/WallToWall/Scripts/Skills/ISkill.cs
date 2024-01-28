using System;

public interface ISkill
{
    event Action OnSkillRelease;
    void Initialize(ISkillRelease skillRelease);
    void ReleaseSkill();
    SkillDataConfig GetSkillDataConfig();
}