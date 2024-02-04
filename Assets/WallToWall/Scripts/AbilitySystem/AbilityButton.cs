using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour
{
    public Image radialProgress;
    public Image abilityImage;

    public int abilityIndex;
    public event Action<int> OnAbilityButtonClicked;

    public void Initialize(int index)
    {
        abilityIndex = index;
    }

    public void RegisterListener(Action<int> action)
    {
        OnAbilityButtonClicked += action;
    }

    public void UnregisterListener(Action<int> action)
    {
        OnAbilityButtonClicked -= action;
    }

    public void UpdateAbilitySprite(Sprite sprite)
    {
        if (abilityImage) abilityImage.sprite = sprite;
    }

    public void UpdateAbilityProgress(float progress)
    {
        if (radialProgress) radialProgress.fillAmount = progress;
    }

    public void OnClick()
    {
        OnAbilityButtonClicked?.Invoke(abilityIndex);
    }

    private void OnDestroy()
    {
        OnAbilityButtonClicked = null;
    }
}