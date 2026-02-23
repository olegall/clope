namespace clope;

internal class ClopeArch
{
    private const double r = 2.6;
    public List<ClusterArch> Clusterize(List<int[]> transactions, double r)
    {
        var dt1 = DateTime.Now;
        var clusters = new List<ClusterArch>();
        AddNewCluster(clusters);
        var cnt = 0;
        // phase 1
        foreach (var tr in transactions)
        {
            cnt++;
            double maxCost = 0;
            var bestChoice = 0;
            for (var i = 0; i < clusters.Count(); i++)
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
        var dt2 = DateTime.Now;
        var phase1 = (dt2 - dt1).TotalSeconds;
        var phase1Ms = (dt2 - dt1).TotalMilliseconds;
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
                var act = clusters.FirstOrDefault(x => x.Transactions.Contains(tr)); // TODO FirstOrDefault vs FindAsync? // зафиксировать индекс кл-ра тр-и
                var actIdx = clusters.IndexOf(act);
                var dr = DeltaRemove(act, tr, r);
                for (var i = 0; i < clusters.Count; i++)
                {
                    if (clusters[i] == act)
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
        var trsCount2 = clusters.SelectMany(x => x.Transactions).Count(); // 8124
        return clusters;
    }

    private double DeltaAdd(ClusterArch C, IEnumerable<int> t, double r)
    {
        if (C.Count == 0) return t.Count() / Math.Pow(t.Count(), r);
        var Snew = C.S + t.Count();
        var Wnew = C.W;
        var hg = C.Histogram; // TODO источник тормозов. возможно тк вычисляет Histogram на лету при обращении к св-ву
        for (int i = 0; i < t.Count() - 1; i++) // TODO источник тормозов. Any
        {
            if (!hg.ContainsKey(t.ElementAt(i))) // TODO occ
            {
                Wnew += 1;
            }
        }
        return Snew * (C.N + 1) / Wnew.P(r) - C.S * C.N / C.W.P(r);
    }

    private double DeltaRemove(ClusterArch C, IEnumerable<int> t, double r)
    {
        var Snew = C.S - t.Count();
        var Wnew = C.W;
        var hg = C.Histogram;
        for (int i = 0; i < t.Count() - 1; i++)
        {
            if (!hg.ContainsKey(t.ElementAt(i)))
            {
                Wnew -= 1;
            }
        }
        return Snew * (C.N - 1) / Wnew.P(r) - C.S * C.N / C.W.P(r);
    }

    private void AddNewCluster(List<ClusterArch> clusters) => clusters.Add(new ClusterArch()); // TODO в Cluster / ClusterService?

    private void RemoveEmptyClusters(ref List<ClusterArch> clusters) => clusters = clusters.Where(x => x.Transactions.Count() > 0).ToList(); // TODO в Cluster / ClusterService? TODO убрать ToList()
}