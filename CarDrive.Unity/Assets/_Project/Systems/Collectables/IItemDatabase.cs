namespace Assets._Project.Systems.Collecting
{
    public interface IItemDatabase
    {
        IItem GetByMergeLevel(ItemType type, int mergeLevel);
        IItem GetRandom();
        IItem GetRandom(int mergeLevel);
        IItem GetByID(string id);
        IItem[] GetByIDs(params string[] ids);
    }
}
