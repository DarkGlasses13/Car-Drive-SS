namespace Assets._Project.Systems.Tutorial
{
    public abstract class TutorialState
    {
        protected ITutorialSystem _system;

        protected TutorialState(ITutorialSystem system)
        {
            _system = system;
        }

        public virtual void Enter() { }
        public virtual void Update() { }
        public virtual void Exit() { }
    }
}
