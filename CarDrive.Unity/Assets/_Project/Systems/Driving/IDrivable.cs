namespace Assets._Project.Systems.Driving
{
    public interface IDrivable
    {
        void RegulateGas(float value);
        void Stear(float value);
    }
}