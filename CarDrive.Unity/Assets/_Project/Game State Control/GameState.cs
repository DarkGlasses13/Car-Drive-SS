namespace Assets._Project.GameStateControl
{
    public class GameState
    {
        public GameStates Current { get; private set; }

        public GameState(GameStates current)
        {
            Current = current;
        }

        public void Switch(GameStates to) => Current = to;
    }
}