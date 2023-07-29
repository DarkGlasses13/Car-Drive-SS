namespace Assets._Project.Systems.Driving
{
    public interface IDrivable
    {
        void Accelerate(float acceleration);
        void ChangeLine(float line, float duration, float stearAngle);
        void SetToLine(float position);
        void Break();
        void EndBreak();
    }
}