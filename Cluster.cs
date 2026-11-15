namespace clope;

internal class Cluster
{
    public Dictionary<int, int> Hist { get; set; }

    /// <summary>
    /// Упорядочить каждую транз-ю - см. картинку кластеров и разбиений над ней https://loginom.ru/blog/clope
    /// TODO IEnumerable<int> Elements
    /// </summary>
    public List<IEnumerable<int>> Transactions { get; set; } = [];

    public int S => Trs.Count();

    //public int W { get => Histogram.Count; }

    public int N => Transactions.Count;

    public IEnumerable<int> Trs => Transactions.SelectMany(x => x);

    public void RecalcAdd(int[] tr)
    {
        if (Hist is null) Hist = [];
        foreach (var item in tr)
        {
            Hist.TryGetValue(item, out int occ); // новый уникальный элемент. c = 0, если гист-ма (словарь) не содержит
            Hist[item] = occ++; // есть ключ el? берём значение и инкрементируем
        }
    }
    
    public void RecalcRemove(int[] tr)
    {
        if (Hist is null) Hist = [];
        foreach (var item in tr)
        {
            Hist.TryGetValue(item, out int occ); // новый уникальный элемент. c = 0, если гист-ма (словарь) не содержит
            Hist[item] = occ--; // есть ключ el? берём значение и инкрементируем
        }
    }
}