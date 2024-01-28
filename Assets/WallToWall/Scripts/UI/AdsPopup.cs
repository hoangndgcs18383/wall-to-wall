using UnityEngine;

public class AdsPopup : BaseScreen
{
    [SerializeField] private ButtonW2W btnClose;

    public override void Initialize()
    {
        base.Initialize();
        btnClose.onClick.AddListener(Hide);
    }
}
