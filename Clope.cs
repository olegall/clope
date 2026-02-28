namespace clope;

internal class Clope
{
    private const double r = 2.6;
    public static List<Cluster> Clusterize(List<int[]> transactions, double r)
    {
        var dt = DateTime.Now;
        var clusters = new List<Cluster>();
        AddNewCluster(clusters);
        var cnt = 0;
        // phase 1
        foreach (var tr in transactions)
        {
            cnt++;
            double maxCost = 0; // maxDelta
            var bestChoice = 0; // j, maxDeltaIdx
            for (var i = 0; i < clusters.Count; i++)
            {
                var da = DeltaAdd(clusters[i], tr, r);
                if (da > maxCost)
                {
                    maxCost = da;
                    bestChoice = i;
                }
            }
            if (clusters[bestChoice].Count == 0) AddNewCluster(clusters);
            clusters[bestChoice].Transactions.Add(tr);
        }
        var phase1 = (DateTime.Now - dt).TotalSeconds; // 47
        var trsCount1 = clusters.SelectMany(x => x.Transactions).Count(); // 8124
        // phase2
        var cnt2 = 0;
        var moved = true;
        while (moved)
        {
            moved = false;
            foreach (var tr in transactions)
            {
                cnt2++;
                double maxCost = 0;
                var bestChoice = 0;
                var act = clusters.First(x => x.Transactions.Contains(tr)); // cluster зафиксировать индекс кл-ра тр-и
                var actIdx = clusters.IndexOf(act);
                
                var dr = DeltaRemove(act, tr);
                for (var i = 0; i < clusters.Count; i++)
                {
                    if (clusters[i] == act) // equals
                    {
                        continue;
                    }

                    var da = DeltaAdd(clusters[i], tr, r);
                    if (da + dr > maxCost)
                    {
                        maxCost = da + dr;
                        bestChoice = i;
                    }
                }
                if (maxCost > 0)
                {
                    if (clusters[bestChoice].Count == 0) AddNewCluster(clusters);

                    clusters[actIdx].Transactions.Remove(tr);
                    clusters[bestChoice].Transactions.Add(tr);
                    moved = true;
                }
            }
        }
        RemoveEmptyClusters(ref clusters);
        var phase2 = (DateTime.Now - dt).TotalSeconds; // 244
        var trsCount2 = clusters.SelectMany(x => x.Transactions).Count(); // 8124
        return clusters;
    }

    private static double DeltaAdd(Cluster C, IEnumerable<int> t, double r)
    {
        if (C.Count == 0) return t.Count() / Math.Pow(t.Count(), r);
        var Snew = C.S + t.Count();
        var Wnew = C.W;
        var hg = C.Histogram; // TODO источник тормозов. возможно тк вычисляет Histogram на лету при обращении к св-ву
        foreach (var el in t)
        {
            if (!hg.ContainsKey(el)) // TODO occ
            {
                Wnew += 1; // ++
            }
        }
        return Snew * (C.N + 1) / Wnew.P(r) - C.S * C.N / C.W.P(r);
    }

    private static double DeltaRemove(Cluster C, IEnumerable<int> t)
    {
        var Snew = C.S - t.Count();
        var Wnew = C.W;
        var hg = C.Histogram;
        foreach (var el in t)
        {
            if (!hg.ContainsKey(el)) // TODO occ
            {
                Wnew -= 1;
            }
        }
        return Snew * (C.N - 1) / Wnew.P(r) - C.S * C.N / C.W.P(r);
    }

    private static void AddNewCluster(List<Cluster> clusters) => clusters.Add(new Cluster()); // TODO в Cluster / ClusterService?

    private static void RemoveEmptyClusters(ref List<Cluster> clusters) => clusters = clusters.Where(x => x.Transactions.Count > 0).ToList(); // TODO в Cluster / ClusterService? TODO убрать ToList()
}