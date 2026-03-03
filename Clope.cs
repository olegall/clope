namespace clope;

internal static class Clope
{
    private const double r = 2.6;
    public static List<Cluster> Clusterize(List<int[]> transactions)
    {
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
            }
            if (clusters[iBestCluster].Count == 0) AddNewCluster(clusters);

            clusters[iBestCluster].Transactions.Add(tr);
        }
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
        return clusters;
    }

    private static double DeltaAdd(Cluster C, int[] t)
    {
        if (C.Count == 0) return t.Length / t.Length.P(r);

        var Snew = C.S + t.Length;
        var Wnew = C.W;
        var hg = C.Histogram;
        foreach (var el in t)
        {
            if (!hg.ContainsKey(el)) Wnew++;
        }
        return Snew * (C.N + 1) / Wnew.P(r) - C.S * C.N / C.W.P(r);
    }

    private static double DeltaRemove(Cluster C, int[] t)
    {
        var Snew = C.S - t.Length;
        var Wnew = C.W;
        var hg = C.Histogram;
        foreach (var el in t)
        {
            if (!hg.ContainsKey(el)) Wnew--;
        }
        return Snew * (C.N - 1) / Wnew.P(r) - C.S * C.N / C.W.P(r);
    }

    private static void AddNewCluster(List<Cluster> clusters) => clusters.Add(new Cluster());

    private static void RemoveEmptyClusters(ref List<Cluster> clusters) => 
        clusters = clusters.Where(x => x.Transactions.Count > 0).ToList();
}