namespace clope;

internal static class Clope
{
    private const double r = 2.6;
    public static List<double> timesLocalMs = [];

    public static List<Cluster> Clusterize(List<int[]> transactions)
    {
        var now = DateTime.Now;

        var clusters = new List<Cluster>();
        #region Phase1
        AddNewCluster(clusters);
        foreach (var tr in transactions)
        {
            //double maxDelta = 0;
            //var iBestCluster = 0;
            //for (var i = 0; i < clusters.Count; i++)
            //{
            //    var da = DeltaAdd(clusters[i], tr);
            //    if (da > maxDelta)
            //    {
            //        maxDelta = da;
            //        iBestCluster = i;
            //    }
            //}
            var iBestCluster = clusters.IndexOf(clusters.MaxBy(x => DeltaAdd(x, tr)));
            if (clusters[iBestCluster].Count == 0) AddNewCluster(clusters);

            clusters[iBestCluster].Transactions.Add(tr);
        }
        //timesLocalMs = [.. timesLocalMs.OrderByDescending(x => x)];
        var sum = timesLocalMs.Sum()/1000;
        var avg = timesLocalMs.Average(x => x);
        var total = (DateTime.Now - now).TotalSeconds;
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
        var sum2 = timesLocalMs.Sum();
        var a1 = clusters.Select(x => x.Transactions);
        return clusters;
    }

    private static double DeltaAdd(Cluster C, int[] t)
    {
        if (C.Count == 0) return t.Length / t.Length.P(r);

        var Snew = C.S + t.Length;
        var hg = C.Histogram(); // value напрямую не учитывается, учитывается группировка (в итоге value), следствием которой есть разный Count
        //var Wnew = C.W;

        var Wnew = hg.Count;
        foreach (var el in t)
        {
            if (!hg.ContainsKey(el)) Wnew++;
        }
        var res = Snew * (C.N + 1) / Wnew.P(r) - C.S * C.N / hg.Count/*C.W*/.P(r);
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

    private static double DeltaRemove(Cluster C, int[] t)
    {
        var Snew = C.S - t.Length;
        var hg = C.Histogram();
        //var Wnew = C.W;
        var Wnew = hg.Count;
        foreach (var el in t)
        {
            if (!hg.ContainsKey(el)) Wnew--;
        }
        var res = Snew * (C.N - 1) / Wnew.P(r) - C.S * C.N / hg.Count/*C.W*/.P(r);
        return res;
    }

    private static void AddNewCluster(List<Cluster> clusters) => clusters.Add(new Cluster());

    private static void RemoveEmptyClusters(ref List<Cluster> clusters) => 
        clusters = clusters.Where(x => x.Transactions.Count > 0).ToList();
}