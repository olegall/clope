namespace clope;

internal class Cluster
{
    /// <summary>
    /// Упорядочить каждую транз-ю - см. картинку кластеров и разбиений над ней https://loginom.ru/blog/clope
    /// TODO IEnumerable<int> Elements
    /// </summary>
    public List<IEnumerable<int>> Transactions { get; set; } = [];

    public int S => Trs.Count();

    //public int W { get => Histogram.Count; }

    public int N => Transactions.Count;

    public IEnumerable<int> Trs => Transactions.SelectMany(x => x);
}