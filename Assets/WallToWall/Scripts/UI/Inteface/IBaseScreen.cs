
public interface IUIData
{
}

public interface IBaseScreen
{
    void Initialize();
    void Show(IUIData data = null);
    void Hide();
    void BackToScreen();
}