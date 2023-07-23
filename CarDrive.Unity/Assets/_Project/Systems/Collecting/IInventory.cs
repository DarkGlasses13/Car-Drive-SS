namespace Assets._Project.Systems.Collecting
{
    public interface IInventory
    {
        int Capacity { get; }
        int Length { get; }

        bool TryAdd(IItem item);
        bool TryRemove(int slot);
        bool TrySwap(int from, int to);
    }
}