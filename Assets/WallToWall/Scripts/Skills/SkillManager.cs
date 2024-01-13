using System.Collections.Generic;

public class SkillManager
{
    private static SkillManager _instance;

    public static SkillManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SkillManager();
            }

            return _instance;
        }
    }

    private Dictionary<string, ISkill> _skills = new Dictionary<string, ISkill>();
    private ISkill _currentSkill;

    public void RegisterSkill(string key, ISkill skill)
    {
        if (_skills.ContainsKey(key))
        {
            return;
        }

        _skills.Add(key, skill);
    }

    public ISkill GetSkill(string key)
    {
        if (_skills.ContainsKey(key))
        {
            return _skills[key];
        }

        return null;
    }
    
    public void SetCurrentSkill(ISkill skill)
    {
        _currentSkill = skill;
    }
    
    public ISkill GetCurrentSkill()
    {
        return _currentSkill;
    }
}