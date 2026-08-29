namespace clope;

internal static class Clope
{
    private const double r = 2.6;
    private static List<double> timesMs = [];

    public static List<Cluster> Clusterize(List<int[]> transactions)
    {
        var start = DateTime.Now;

        var clusters = new List<Cluster>();
        #region Phase1
        AddNewCluster(clusters);
        foreach (var tr in transactions)
        {
            double maxDelta = 0;
            var iBestCluster = 0;
            for (var i = 0; i < clusters.Count; i++)
            {
                var da = DeltaAdd(clusters[i], tr);
                if (da > maxDelta)
                {
                    maxDelta = da;
                    iBestCluster = i;
                }
                //Console.Debug("***");
                //Debug.WriteLine(end2);
            }
            if (clusters[iBestCluster].Count == 0) AddNewCluster(clusters);

            clusters[iBestCluster].Transactions.Add(tr);
        }
        timesMs = [.. timesMs.OrderByDescending(x => x)];
        var sum = timesMs.Sum();
        var end = DateTime.Now - start;
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
                var actIdx = clusters.IndexOf(act);
                
                var dr = DeltaRemove(act, tr);
                for (var i = 0; i < clusters.Count; i++)
                {
                    if (clusters[i] == act) continue;

                    var da = DeltaAdd(clusters[i], tr);
                    if (da + dr > maxDelta)
                    {
                        maxDelta = da + dr;
                        iBestCluster = i;
                    }
                }
                if (maxDelta > 0)
                {
                    if (clusters[iBestCluster].Count == 0) AddNewCluster(clusters);

                    clusters[actIdx].Transactions.Remove(tr);
                    clusters[iBestCluster].Transactions.Add(tr);
                    moved = true;
                }
            }
        }
        #endregion
        RemoveEmptyClusters(ref clusters);
        var sum2 = timesMs.Sum();
        var a1 = clusters.Select(x => x.Transactions);
        return clusters;
    }

    private static double DeltaAdd(Cluster C, int[] t)
    {
        var dt = DateTime.Now;
        if (C.Count == 0) return t.Length / t.Length.P(r);

        var Snew = C.S + t.Length;
        var hg = C.Histogram;
        //var Wnew = C.W;
        var Wnew = hg.Count;
        foreach (var el in t)
        {
            if (!hg.ContainsKey(el)) Wnew++;
        }
        var res = Snew * (C.N + 1) / Wnew.P(r) - C.S * C.N / hg.Count.P(r);
        timesMs.Add((DateTime.Now - dt).TotalMilliseconds);
        return res;
    }

    private static double DeltaRemove(Cluster C, int[] t)
    {
        var dt = DateTime.Now;
        var Snew = C.S - t.Length;
        var hg = C.Histogram;
        //var Wnew = C.W;
        var Wnew = hg.Count;
        foreach (var el in t)
        {
            if (!hg.ContainsKey(el)) Wnew--;
        }
        var res = Snew * (C.N - 1) / Wnew.P(r) - C.S * C.N / hg.Count.P(r);
        timesMs.Add((DateTime.Now - dt).TotalMilliseconds);
        return res;
    }

    private static void AddNewCluster(List<Cluster> clusters) => clusters.Add(new Cluster());

    private static void RemoveEmptyClusters(ref List<Cluster> clusters) => 
        clusters = clusters.Where(x => x.Transactions.Count > 0).ToList();
}