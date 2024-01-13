using System;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public struct SkillDataConfig
{
    public string NameDisplay;
    [TextArea]
    public string Description;
    public bool IsUseEffect;
    [ShowIf("IsUseEffect")] [Required]
    public GameObject Effect;
    public bool IsUseSprite;
    [ShowIf("IsUseSprite")] [Required]
    public Sprite Sprite;
    public float Duration;
    public float CoolDown;
}
