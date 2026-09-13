namespace clope;

internal class Cluster
{
    public List<IEnumerable<int>> Transactions { get; set; } = [];

    public Dictionary<int, int> Histogram { get => Trs.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count()); }
    //public Dictionary<int, int> Histogram()
    //{
    //    var grouped = Trs.GroupBy(x => x);
    //    var now = DateTime.Now;
    //    var dict = grouped.ToDictionary(x => x.Key, x => x.Count());
    //    Clope.timesLocalMs.Add((DateTime.Now - now).TotalMilliseconds);
    //    return dict;
    //}

    public IEnumerable<IGrouping<int, int>> Grouped() => Trs.GroupBy(x => x);

    public int[] GroupedArr()
    {
        List<List<int>> result = new List<List<int>>();
        List<int> currentGroup = null;

        foreach (var item in Trs)
        {
            if (currentGroup == null || currentGroup[0] != item)
            {
                currentGroup = new List<int>();
                result.Add(currentGroup);
            }
            currentGroup.Add(item);
        }
        return result.SelectMany(x => x).ToArray();
    }

    public int Count => Transactions.Count;

    public int S => Trs.Count();

    public int W { get => Histogram.Count; }

    public int N => Transactions.Count;

    private IEnumerable<int> Trs => Transactions.SelectMany(x => x);
}