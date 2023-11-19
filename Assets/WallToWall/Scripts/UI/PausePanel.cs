public class PausePanel : BaseScreen
{
    public ButtonW2W resumeButton;

    public override void Initialize()
    {
        base.Initialize();
        resumeButton.onClick.AddListener(OnResumeButton);
    }

    public override void Show(IUIData data = null)
    {
        base.Show(data);
        GameManager.Instance.PauseGame();
    }

    private void OnResumeButton()
    {
        GameManager.Instance.ResumeGame();
        Hide();
    }
}