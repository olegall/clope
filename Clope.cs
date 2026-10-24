using System.Transactions;

namespace clope;

internal static class Clope
{
    private const double rep = 2.6;
    public static List<double> timesLocalMs = [];
    private static readonly Dictionary<int, Dictionary<int, int>> _histogram = [];
    private static readonly List<int>[] _histograms = new List<int>[1000];
    public static List<Cluster> Clusterize(List<int[]> transactions)
    {
        var now = DateTime.Now;
        var clusters = new List<Cluster>();
        #region Phase1
        AddNewCluster(clusters);
        foreach (var tr in transactions)
        {
            double maxDelta = 0;
            var iBestCluster = 0;
            for (var i = 0; i < clusters.Count; i++)
            {
                var da = DeltaAdd(clusters[i], tr, i);
                if (da > maxDelta)
                {
                    maxDelta = da;
                    iBestCluster = i;
                }
            }
            //var iBestCluster = clusters.IndexOf(clusters.MaxBy(x => DeltaAdd(x, tr)));

            if (clusters[iBestCluster].N == 0) AddNewCluster(clusters);

            clusters[iBestCluster].Transactions.Add(tr);
        }
        var sum = timesLocalMs.Sum()/1000;
        var total = (DateTime.Now - now).TotalSeconds;
        //Test(clusters);
        #endregion
        #region Phase2
        var moved = true;
        while (moved)
        {
            moved = false;
            foreach (var tr in transactions)
            {
                double maxDelta = 0;
                var iBestCluster = 0;
                var act = clusters.First(x => x.Transactions.Contains(tr));

                var dr = DeltaRemove(act, tr);
                for (var i = 0; i < clusters.Count; i++)
                {
                    if (clusters[i] == act) continue;

                    var da = DeltaAdd(clusters[i], tr, i);
                    if (da + dr > maxDelta)
                    {
                        maxDelta = da + dr;
                        iBestCluster = i;
                    }
                }
                // TODO преобразовать в словарь, искать по и-су
                //var iBestCluster = clusters.IndexOf(clusters.Where(x => x != act).MaxBy(x => { maxDelta = DeltaAdd(x, tr) + DeltaRemove(act, tr); return maxDelta; }));
                if (maxDelta > 0)
                {
                    if (clusters[iBestCluster].N == 0) AddNewCluster(clusters);

                    var actIdx = clusters.IndexOf(act);
                    clusters[actIdx].Transactions.Remove(tr);
                    clusters[iBestCluster].Transactions.Add(tr);
                    moved = true;
                }
            }
        }
        #endregion
        RemoveEmptyClusters(ref clusters);
        var sum2 = timesLocalMs.Sum()/1000;
        var total2 = (DateTime.Now - now).TotalSeconds;
        Test(clusters);
        return clusters;
    }

    private static double DeltaAdd(Cluster C, int[] tr, int clusterId)
    {
        if (C.N == 0) return tr.Length / tr.Length.P(rep);

        var Snew = C.S + tr.Length;
        #region пересчёт гистограммы
        var hg = C.Histogram6(); // value напрямую не учитывается, учитывается группировка (в итоге value), следствием которой есть разный Count
        //var Wnew = C.W;

        var Wnew = hg.Count();
        foreach (var el in tr)
        {
            if (!hg.Contains(el)) Wnew++;
        }
        #endregion
        //#region Dictionary
        //if (!_histogram.TryGetValue(clusterId, out var hist))
        //    _histogram[clusterId] = hist = [];
        ////foreach (var item in C.Transactions.SelectMany(x => x))
        //foreach (var item in tr)
        //{
        //    hist.TryGetValue(item, out int c); // новый уникальный элемент. c = 0, если гист-ма (словарь) не содержит
        //    hist[item] = c + 1; // есть ключ item? берём значение и инкрементируем
        //}
        // в watch hg и _histogram в _histogram д меняться только тот эл-т, который меняется _histogram - это список списков
        //var Wnew = _histogram[clusterId].Keys.Count();
        //foreach (var el in tr)
        //{
        //    if (!_histogram[clusterId].Keys.Contains(el)) Wnew++;
        //}
        //#endregion
        //#region Array Of Lists
        //if (_histograms[clusterId] is null)
        //    _histograms[clusterId] = new HashSet<int>(C.Transactions.SelectMany(x => x)).ToList();
        //else
        //{
        //    var now = DateTime.Now;
        //    foreach (var item in C.Transactions.SelectMany(x => x))
        //    //foreach (var tr_ in C.Transactions)
        //    //    foreach (var item in tr)
        //    if (!_histograms[clusterId].Contains(item)) _histograms[clusterId].Add(item);
        //    timesLocalMs.Add((DateTime.Now - now).TotalMilliseconds);
        //}
        //var Wnew = _histograms[clusterId].Count;
        //foreach (var el in tr)
        //{
        //    if (!_histograms[clusterId].Contains(el)) Wnew++;
        //}
        //#endregion


        var res = Snew * (C.N + 1) / Wnew.P(rep) - C.S * C.N / hg.Count()/*C.W*/.P(rep);
        //var res = Snew * (C.N + 1) / Wnew.P(rep) - C.S * C.N / _histogram[clusterId].Count().P(rep);
        //var res = Snew * (C.N + 1) / Wnew.P(rep) - C.S * C.N / _histograms[clusterId].Count.P(rep);
        //var res = Snew * (C.N + 1) / Math.Pow(Wnew, r) - C.S * C.N / Math.Pow(hg.Count/*C.W*/, r);
        return res;
    }

    //private static double DeltaAdd(Cluster C, int[] t)
    //{
    //    var now = DateTime.Now;
    //    if (C.Count == 0) return t.Length / t.Length.P(r);

    //    var Snew = C.S + t.Length;
    //    var hg = C.Grouped().ToArray();
    //    //var Wnew = C.W;

    //    var keys = hg.Select(x => x.Key).ToArray();
    //    var Wnew = keys.Length;

    //    foreach (var el in t)
    //    {
    //        if (!keys.Contains(el)) Wnew++;
    //    }
    //    var res = Snew * (C.N + 1) / Wnew.P(r) - C.S * C.N / hg.Length/*C.W*/.P(r);
    //    timesLocalMs.Add((DateTime.Now - now).TotalMilliseconds);
    //    timesLocalMs = [.. timesLocalMs.OrderByDescending(x => x)];
    //    return res;
    //}

    //private static double DeltaAdd(Cluster C, int[] t)
    //{
    //    var now = DateTime.Now;
    //    if (C.Count == 0) return t.Length / t.Length.P(r);

    //    var Snew = C.S + t.Length;
    //    var hg = C.GroupedArr();
    //    //var Wnew = C.W;

    //    var keys = hg;
    //    var Wnew = keys.Length;

    //    foreach (var el in t)
    //    {
    //        if (!keys.Contains(el)) Wnew++;
    //    }
    //    var res = Snew * (C.N + 1) / Wnew.P(r) - C.S * C.N / hg.Length/*C.W*/.P(r);
    //    timesLocalMs.Add((DateTime.Now - now).TotalMilliseconds);
    //    return res;
    //}

    private static double DeltaRemove(Cluster C, int[] tr)
    {
        var Snew = C.S - tr.Length;
        var hg = C.Histogram6();
        //var Wnew = C.W;
        var Wnew = hg.Count();
        foreach (var el in tr)
        {
            if (!hg.Contains(el)) Wnew--;
        }
        var res = Snew * (C.N - 1) / Wnew.P(rep) - C.S * C.N / hg.Count()/*C.W*/.P(rep);
        return res;
    }

    private static void AddNewCluster(List<Cluster> clusters) => clusters.Add(new Cluster());

    private static void RemoveEmptyClusters(ref List<Cluster> clusters) => 
        clusters = clusters.Where(x => x.N > 0).ToList();

    private static void Test(List<Cluster> clusters)
    {
        var strs = new List<string>();
        foreach (var cl in clusters)
        {
            var str = "";
            foreach (var tr in cl.Transactions)
            {
                foreach (var el in tr)
                {
                    str += el < 10 ? "0" + el + " " : el + " ";
                }
                str += Environment.NewLine;
            }
            strs.Add(str);
        }
        //clusters[0].Transactions.Select(tr => tr.Select(el => el))
    }
}