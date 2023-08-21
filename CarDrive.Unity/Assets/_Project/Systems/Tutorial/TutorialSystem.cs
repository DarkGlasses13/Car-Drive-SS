using Assets._Project.Architecture;
using Assets._Project.GameStateControl;
using System.Collections.Generic;

namespace Assets._Project.Systems.Tutorial
{
    public class TutorialSystem : GameSystem, ITutorialSystem
    {
        private GameState _gameState;
        private Queue<TutorialState> _states;

        public TutorialSystem(GameState gameState)
        {
            _gameState = gameState;
        }

        public void AddStates(params TutorialState[] states) => _states = new(states);

        public override void Initialize()
        {
            _states.Peek().Enter();
        }

        public void SwitchToNextState()
        {
            _states.Dequeue().Exit();
            _states.Peek().Enter();
        }

        public override void Tick()
        {
            _states.Peek().Update();
        }
    }
}
