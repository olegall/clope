namespace clope;

internal class Cluster
{
    private readonly List<IEnumerable<int>> _C;
    //public Cluster(List<IEnumerable<int>> C)
    //{
    //    _C = C;
    //}

    public List<IEnumerable<int>> Transactions { get; set; }

    public Dictionary<int, int> Histogram
    {
        get
        {
            var histogram = Trs.GroupBy(x => x); //var histogram = trs.GroupBy(x => x).ToDictionary<int, double>();
            var histogram_ = new Dictionary<int, int>(); // <TOcc, TObject>
            foreach (var item in histogram.ToArray()) histogram_.Add(item.Key, item.Count()); //histogram.Select(x => histogram_.Add(x.Key, x.Count()));
            return histogram_;
        }
    }

    public int Count => _C.Count();
    public int S => Trs.Count();
    public int W => Histogram.Count();

    public int N => _C.Count();

    private IEnumerable<int> Trs => _C.SelectMany(x => x);
}

internal class ClusterArch
{
    public List<IEnumerable<int>> Transactions { get; set; } = new List<IEnumerable<int>>();

    public Dictionary<int, int> Histogram
    {
        get
        {
            var histogram = Trs.GroupBy(x => x); //var histogram = trs.GroupBy(x => x).ToDictionary<int, double>();
            var histogram_ = new Dictionary<int, int>(); // <TOcc, TObject>
            foreach (var item in histogram.ToArray()) histogram_.Add(item.Key, item.Count()); //histogram.Select(x => histogram_.Add(x.Key, x.Count()));
            return histogram_;
        }
    }

    public int Count => Transactions.Count();
    public int S => Trs.Count();
    public int W => Histogram.Count();

    public int N => Transactions.Count();

    private IEnumerable<int> Trs => Transactions.SelectMany(x => x);
}

internal static class C
{
    public static List<IEnumerable<int>> self;
    //static C_(List<IEnumerable<int>> C)
    //{
    //    _C = C;
    //}

    public static Dictionary<int, int> Histogram
    {
        get
        {
            var histogram = Trs.GroupBy(x => x); //var histogram = trs.GroupBy(x => x).ToDictionary<int, double>();
            var histogram_ = new Dictionary<int, int>(); // <TOcc, TObject>
            foreach (var item in histogram.ToArray()) histogram_.Add(item.Key, item.Count()); //histogram.Select(x => histogram_.Add(x.Key, x.Count()));
            return histogram_;
        }
    }

    public static int S => Trs.Count();

    public static int W => Histogram.Count();

    public static int N => self.Count();

    private static IEnumerable<int> Trs => self.SelectMany(x => x);
}