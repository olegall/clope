namespace clope;

public class Clope_
{
    private readonly Dictionary<int, Dictionary<int, int>> _histogram;
    // clusterId -> (item -> count)

    private readonly Dictionary<int, int> _clusterSizes;  // clusterId -> |D|
    private readonly Dictionary<int, int> _clusterWidths; // clusterId -> W

    public Clope_(int itemCount)
    {
        _histogram = new Dictionary<int, Dictionary<int, int>>();
        _clusterSizes = new Dictionary<int, int>();
        _clusterWidths = new Dictionary<int, int>();
    }

    // Инкрементальное добавление транзакции в кластер
    private void AddToCluster(int clusterId, int[] items)
    {
        if (!_histogram.TryGetValue(clusterId, out var hist))
            _histogram[clusterId] = hist = new Dictionary<int, int>();

        foreach (var item in items)
        {
            hist.TryGetValue(item, out int c);
            hist[item] = c + 1;
        }
        _clusterSizes[clusterId] = _clusterSizes.GetValueOrDefault(clusterId) + 1;
        _clusterWidths[clusterId] = hist.Count; // ширина = число уникальных item
    }

    // Инкрементальное удаление
    private void RemoveFromCluster(int clusterId, int[] items)
    {
        var hist = _histogram[clusterId];
        foreach (var item in items)
        {
            if (--hist[item] == 0)
                hist.Remove(item);
        }
        _clusterSizes[clusterId]--;
        _clusterWidths[clusterId] = hist.Count;
    }

    //private double Profit(int clusterId) =>
    //    _clusterSizes[clusterId] / Math.Pow(_clusterWidths[clusterId], Repulsion);
}
