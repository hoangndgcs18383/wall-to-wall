using System.Collections.Generic;
using UnityEngine;

public class RatePanel : BaseScreen
{
    public bool isHideInAwake;

    [SerializeField] private RateStar starsPrefab;
    [SerializeField] private RectTransform starsContainer;

    private List<RateStar> _stars = new List<RateStar>();

    private void Awake()
    {
        if (isHideInAwake && gameObject.activeInHierarchy && gameObject.activeSelf)
        {
            Hide();
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        _stars.Clear();
        for (int i = 0; i < 5; i++)
        {
            RateStar r = Instantiate(starsPrefab, starsContainer);
            r.Initialize(OnRate, (i, 0), OnClick);
            _stars.Add(r);
        }
    }

    private void OnClick((int, float) obj)
    {
    }

    private void OnRate((int r, float p) rate)
    {
        for (int i = 0; i < _stars.Count; i++)
        {
            _stars[i].SetFill(0);
        }

        for (int i = 0; i < rate.r; i++)
        {
            _stars[i].SetFill(1);
        }

        _stars[rate.r].SetFill(rate.p);
    }

    public void OnClickRateUs()
    {
        Debug.Log("Rate Us");
    }
}