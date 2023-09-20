using Assets._Project.Architecture;
using Assets._Project.GameStateControl;
using Assets._Project.Systems.Collecting;
using System.Collections.Generic;

namespace Assets._Project.Systems.Tutorial
{
    public class TutorialSystem : GameSystem, ITutorialSystem
    {
        private readonly GameState _gameState;
        private readonly IInventory _inventory;
        private readonly Money _money;
        private readonly Player _player;
        private Queue<TutorialState> _states;

        public TutorialSystem(GameState gameState, IInventory inventory, Money money, Player player)
        {
            _gameState = gameState;
            _inventory = inventory;
            _money = money;
            _player = player;
        }

        public void AddStates(params TutorialState[] states) => _states = new(states);

        public void Start()
        {
            if (_states.Count > 0)
                _states.Peek().Enter();


            _player.ResetSats();
            _inventory?.Clear();
            _money.SpendAll();
        }

        public void SwitchToNextState()
        {
            if (_states.Count > 0)
            {
                _states.Dequeue().Exit();
                _states.Peek().Enter();
            }
        }

        public override void Tick()
        {
            if (_states.Count > 0)
                _states.Peek().Update();
        }

        public override void OnDisable()
        {
            if (_states.Count > 0)
                _states.Dequeue().Exit();

            _states.Clear();
        }
    }
}
