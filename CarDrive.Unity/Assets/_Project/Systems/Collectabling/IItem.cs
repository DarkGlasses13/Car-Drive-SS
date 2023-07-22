namespace Assets._Project.Systems.Collectabling
{
    public interface IItem
    {
        string ID { get; }
        string Title { get; }
        string Description { get; }
    }
}