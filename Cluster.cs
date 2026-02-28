namespace clope;

internal class Cluster
{
    public List<IEnumerable<int>> Transactions { get; set; } = [];

    public Dictionary<int, int> Histogram { get => Trs.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count()); } // get

    public int Count { get => Transactions.Count; }

    public int S { get => Trs.Count(); }

    public int W { get => Histogram.Count; }

    public int N { get => Transactions.Count; }

    private IEnumerable<int> Trs { get => Transactions.SelectMany(x => x); }
}