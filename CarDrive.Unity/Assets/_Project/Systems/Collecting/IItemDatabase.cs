namespace Assets._Project.Systems.Collecting
{
    public interface IItemDatabase
    {
        IItem GetRandom();
        bool TryGetByID(string id, out IItem item);
    }
}
