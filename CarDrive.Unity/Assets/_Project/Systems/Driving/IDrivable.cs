namespace Assets._Project.Systems.Driving
{
    public interface IDrivable
    {
        void Accelerate(float acceleration);
        void ChangeLine(float shift, float duration, float stearAngle);
    }
}