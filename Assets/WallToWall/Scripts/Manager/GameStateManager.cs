public class GameStateManager : Singleton<GameStateManager>
{
    public enum GameState
    {
        MainMenu,
        InGame,
        Pause,
        GameOver
    }

    public GameState CurrentGameState { get; private set; }

    public void SetState(GameState state)
    {
        CurrentGameState = state;
    }
}