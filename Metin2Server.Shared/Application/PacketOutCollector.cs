namespace Metin2Server.Shared.Application;

public class PacketOutCollector
{
    private readonly List<object> _items = new();
    public IReadOnlyList<object> Items => _items;

    public void Add<T>(T dto) => _items.Add(dto!);
    public void AddRange(IEnumerable<object> items) => _items.AddRange(items);
    public void Clear() => _items.Clear();
}