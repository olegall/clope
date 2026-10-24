namespace clope;

internal class Cluster
{
    /// <summary>
    /// Упорядочить каждую транз-ю - см. картинку кластеров и разбиений над ней https://loginom.ru/blog/clope
    /// TODO IEnumerable<int> Elements
    /// </summary>
    public List<IEnumerable<int>> Transactions { get; set; } = [];

    //private HashSet<int> HashSet = new HashSet<int>();

    //public Dictionary<int, int> Histogram { get => Trs.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count()); }
    public Dictionary<int, int> Histogram()
    {
        var grouped = Trs.GroupBy(x => x);
        var now = DateTime.Now;
        var dict = grouped.ToDictionary(x => x.Key, x => x.Count());
        Clope.timesLocalMs.Add((DateTime.Now - now).TotalMilliseconds);
        return dict;
    }

    public IEnumerable<int> Histogram4()
    {
        var grouped = Trs.GroupBy(x => x); // .ToArray() - задержка
        var res = grouped.Select(x => x.ElementAt(0));
        // IEnumerable - почти нет задержки, запрос дёргается (рез-т выполняется) в вызывающем коде - hg.Contains
        // List - eager loading, есть ощутимая зарержка
        return res;
    }

    //public List<int> Histogram4()
    //{
    //    var grouped = Trs.GroupBy(x => x);
    //    var res = grouped.Select(x => x.ElementAt(0)).ToList();
    //    return res;
    //}

    public IEnumerable<IGrouping<int, int>> Grouped() => Trs.GroupBy(x => x);

    public void Histogram2()
    {
        var res = new List<int>();
        var coll = Trs.GroupBy(x => x);
        //foreach (var item in coll) res.Add(item.Sum(x => x));
        foreach (var item in coll) res.Add(item.Select(x => x).ElementAt(0));
        //var dict_ = Trs.GroupBy(x => x).Select(x => x.Select);
    }

    public void Histogram3() 
    {
        var trs_ = Trs.OrderBy(x => x).ToArray();
        var result = new List<int>();
        for (int i = 0; i < trs_.Length - 1; i++)
        {
            if (trs_[i+1] != trs_[i]) result.Add(trs_[i]);
        }
    }
    
    public void Histogram5()
    {
        var trs_ = Trs.OrderBy(x => x).ToArray();
        var result = new int[trs_.Length];
        for (int i = 0; i < trs_.Length - 1; i++)
        {
            if (trs_[i+1] != trs_[i]) result[i] = trs_[i];
        }
    }

    /// <summary>
    /// TODO int[] Histogram6(), в DeltaAdd hg.Length
    /// TODO класс гистограммы, в ней транзакции
    /// </summary>
    public IEnumerable<int> Histogram6()
    {
        
        //var res = Trs.Distinct().ToArray();
        var res = new HashSet<int>(Transactions.SelectMany(x => x)); // уникальные эл-ты
        
        return res;
    }

    public int S => Trs.Count();

    //public int W { get => Histogram.Count; }

    public int N => Transactions.Count;

    private IEnumerable<int> Trs => Transactions.SelectMany(x => x);
}