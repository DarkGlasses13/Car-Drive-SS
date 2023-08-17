using Assets._Project.Architecture;

namespace Assets._Project.Input
{
    public class InputSystem : GameSystem
    {
        private readonly IPlayerInputHandler _inputHandler;

        public InputSystem(IPlayerInputHandler inputHandler)
        {
            _inputHandler = inputHandler;
        }

        public override void Tick()
        {
            _inputHandler?.Read();
        }
    }
}
