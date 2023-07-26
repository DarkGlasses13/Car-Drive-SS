namespace Assets._Project.Systems.Collecting
{
    public interface IItemDatabase
    {
        IItem GetByMergeLevel(ItemType type, int mergeLevel);
        IItem GetRandom();
        IItem GetRandom(int mergeLevel);
        bool TryGetByID(string id, out IItem item);
    }
}
