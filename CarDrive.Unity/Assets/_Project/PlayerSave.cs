using System;

namespace Assets._Project
{
    [Serializable]
    public struct PlayerSave
    {
        public int Level;
        public bool IsTutorialCompleted;
        public int Money;
        public string[] Equipment;
        public string[] Items;
        public float[] Stats;
        public bool IsSound;
    }
}
