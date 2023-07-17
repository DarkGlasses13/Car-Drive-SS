namespace Assets._Project.Input
{
    public class PlayerInput : IPlayerInput
    {
        private readonly IPlayerInputConfig _config;

        public float GasRegulation { get; private set; }
        public float Stear { get; private set; }

        public PlayerInput(IPlayerInputConfig config)
        {
            _config = config;
        }

        public void Tick()
        {
            GasRegulation = _config.GasRegulationInputAction.ReadValue<float>();
            Stear = _config.StearInputAction.ReadValue<float>();
        }
    }
}
