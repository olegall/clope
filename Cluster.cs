namespace clope;

internal class Cluster
{
    public List<IEnumerable<int>> Transactions { get; set; } = new List<IEnumerable<int>>();

    public Dictionary<int, int> Histogram => Trs.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());

    public int Count => Transactions.Count();
    public int S => Trs.Count();
    public int W => Histogram.Count();

    public int N => Transactions.Count();

    private IEnumerable<int> Trs => Transactions.SelectMany(x => x);
}