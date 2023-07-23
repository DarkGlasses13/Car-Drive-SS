namespace Assets._Project.Systems.Collecting
{
    public interface IItemDatabase
    {
        bool TryGetByID(string id, out IItem item);
    }
}
