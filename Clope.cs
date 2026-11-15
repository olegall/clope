namespace clope;

internal static class Clope
{
    private const double rep = 2.6;
    public static List<double> timesLocalMs = [];
    private static readonly Dictionary<int, Dictionary<int, int>> _clusterIdToHist = []; // Dictionary<ClusterId, Dictionary<item, Occ>>
    //private static readonly List<int>[] _histograms = new List<int>[1000];
    public static List<Cluster> Clusterize(List<int[]> transactions)
    {
        var now = DateTime.Now;
        var clusters = new List<Cluster>();
        #region Phase1
        AddNewCluster(clusters);
        foreach (var tr in transactions) // for по кластерам внешний, for по транзакциям внутренний
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

            AddNewClusterIfBestIsEmpty(clusters, iBestCluster);

            clusters[iBestCluster].Transactions.Add(tr); //i=0
            if (!_clusterIdToHist.TryGetValue(iBestCluster, out var hist)) _clusterIdToHist[iBestCluster] = hist = []; // информация о транзакциях не сохраняется. она в кластере
            foreach (var item in tr)
            {
                hist.TryGetValue(item, out int occ); // новый уникальный элемент. c = 0, если гист-ма (словарь) не содержит
                hist[item] = occ++; // есть ключ el? берём значение и инкрементируем
            }
        }
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
                var act = clusters.First(x => x.Transactions.Contains(tr)); // какой-то кластер в любом случае будет содержать тр-ю, т.к. кластеры сформированы тр-ми в фазе 1
                var actIdx = clusters.IndexOf(act);
                var dr = DeltaRemove(act, tr, actIdx);
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
                    AddNewClusterIfBestIsEmpty(clusters, iBestCluster);

                    act.Transactions.Remove(tr);
                    if (!_clusterIdToHist.TryGetValue(actIdx, out var histRm)) _clusterIdToHist[actIdx] = histRm = [];
                    foreach (var item in tr)
                    {
                        histRm.TryGetValue(item, out int occ); // Occ новый уникальный элемент. c = 0, если гист-ма (словарь) не содержит
                        histRm[item] = occ--/*++*/; // есть ключ el? берём значение и инкрементируем
                    }
                    clusters[iBestCluster].Transactions.Add(tr);
                    if (!_clusterIdToHist.TryGetValue(iBestCluster, out var hist)) _clusterIdToHist[iBestCluster] = hist = [];
                    foreach (var item in tr)
                    {
                        hist.TryGetValue(item, out int occ); // новый уникальный элемент. c = 0, если гист-ма (словарь) не содержит
                        hist[item] = occ++; // есть ключ el? берём значение и инкрементируем
                    }
                    moved = true;
                }
            }
        }
        #endregion
        RemoveEmptyClusters(ref clusters);
        var sum2 = timesLocalMs.Sum()/1000;
        var total2 = (DateTime.Now - now).TotalSeconds;
        var a3 = Test(clusters);
        return clusters;
    }

    /// <summary>
    /// Лучший кластер - пустой? (транзакция максимально отличается от транзакций в других кластерах). Текущий пустой кластер теперь задействуется. Нужен новый пустой
    /// TODO неочевидно. Переписать, см. оф доку put t in an existing cluster or a new cluster
    /// </summary>
    private static void AddNewClusterIfBestIsEmpty(List<Cluster> clusters, int iBestCluster)
    {
        if (clusters[iBestCluster].N == 0) AddNewCluster(clusters);
    }

    private static double DeltaAdd(Cluster C, int[] tr, int clusterId)
    {
        if (C.N == 0) return tr.Length / tr.Length.P(rep);

        var Snew = C.S + tr.Length;
        #region Dictionary
        var Wnew2 = _clusterIdToHist[clusterId].Keys.Count();
        var Snew2 = C.S + tr.Length;
        //var Snew2 = _histograms[clusterId].Values.Sum() + tr.Length;
        foreach (var el in tr)
        {
            if (!_clusterIdToHist[clusterId].Keys.Contains(el)) Wnew2++;
        }
        var res = Snew2 * (C.N + 1) / Wnew2.P(rep) - C.S * C.N / _clusterIdToHist[clusterId].Count().P(rep);
        #endregion
        //var res = Snew * (C.N + 1) / Math.Pow(Wnew, r) - C.S * C.N / Math.Pow(hg.Count/*C.W*/, r);
        return res;
    }

    private static double DeltaRemove(Cluster C, int[] tr, int clusterId)
    {
        #region Dictionary
        var Wnew2 = _clusterIdToHist[clusterId].Keys.Count();
        var Snew2 = C.S - tr.Length;
        foreach (var el in tr)
        {
            if (!_clusterIdToHist[clusterId].Keys.Contains(el)) Wnew2--;
        }
        var res = Snew2 * (C.N - 1) / Wnew2.P(rep) - C.S * C.N / _clusterIdToHist[clusterId].Count().P(rep);
        #endregion
        return res;
    }

    private static void AddNewCluster(List<Cluster> clusters) => clusters.Add(new Cluster());

    private static void RemoveEmptyClusters(ref List<Cluster> clusters) => 
        clusters = clusters.Where(x => x.N > 0).ToList();

    private static List<string> Test(List<Cluster> clusters)
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
        return strs;
        //clusters[0].Transactions.Select(tr => tr.Select(el => el))
    }
}