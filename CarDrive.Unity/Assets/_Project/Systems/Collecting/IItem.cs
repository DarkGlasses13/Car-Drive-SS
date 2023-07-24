namespace Assets._Project.Systems.Collecting
{
    public interface IItem
    {
        string ID { get; }
        string Title { get; }
        string Description { get; }
        ItemType Type { get; }
        IItem MergeResult { get; }
    }
}