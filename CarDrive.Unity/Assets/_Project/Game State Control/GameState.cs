using System;

namespace Assets._Project.GameStateControl
{
    public class GameState
    {
        public Action<GameStates> OnSwitched;
        public GameStates Current { get; private set; }

        public GameState(GameStates current)
        {
            Current = current;
        }

        public void Switch(GameStates to)
        {
            Current = to;
            OnSwitched?.Invoke(Current);
        }
    }
}