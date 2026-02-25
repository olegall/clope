namespace clope;

internal class Clope2<TTransaction, TCluster> where TTransaction : IEnumerable<int> where TCluster : List<TTransaction>, new()
{
    private const double r = 2.6;
    public List<List<IEnumerable<int>>> Clusterize(TCluster transactions, double r)
    {
        var dt1 = DateTime.Now;
        #region data
        //List<TCluster> clustersOutput =
        //[
        //    [ ['a', 'b'], [ 'a', 'b', 'c' ], [ 'a', 'c', 'd' ] ], // кластер 1
        //    [ [ 'd', 'e' ], [ 'd', 'e', 'f' ] ] // кластер 2
        //];
        //TTransaction tr1 = new char[] { '1' };
        //char[] tr1_ = { '1' };
        //IEnumerable<char> tr2 = new char[] { '1' };
        //var tr3 = new char[] { '1' };
        //var cl1 = new TCluster();
        //cl1.Add(
        //List<TCluster> clusters =
        //var cls = new List<TCluster>() { new TCluster() { }, new TCluster() };

        //List<List<IEnumerable<char>>> clusters =
        //[
        //    [ new[] { 'a', 'b' }, new[] { 'a', 'b', 'c' } ], // кластер 1. [ 'a', 'b', 'c' ] - IReadOnlyArray, new[] { 'a', 'b' } - просто массив
        //    [ new[] { 'x', 's' }, new[] { 'y', 'w' } ] // кластер 2
        //];
        //List<List<IEnumerable<char>>> clusters = // похожие
        //[
        //    [ new[] { 'p', 'x', 's', 'n', 't', 'p', 'f', 'c', 'n', 'k', 'e', 'e', 's', 's', 'w', 'w', 'p', 'w', 'o', 'p', 'k', 's', 'u' } ],
        //    [ new[] { 'e', 'x', 's', 'y', 't', 'a', 'f', 'c', 'b', 'k', 'e', 'c', 's', 's', 'w', 'w', 'p', 'w', 'o', 'p', 'n', 'n', 'g' } ],
        //    [ new[] { 'e', 'b', 's', 'w', 't', 'l', 'f', 'c', 'b', 'n', 'e', 'c', 's', 's', 'w', 'w', 'p', 'w', 'o', 'p', 'n', 'n', 'm' } ],
        //    [ new[] { 'p', 'x', 'y', 'w', 't', 'p', 'f', 'c', 'n', 'n', 'e', 'e', 's', 's', 'w', 'w', 'p', 'w', 'o', 'p', 'k', 's', 'u' } ],
        //    [ new[] { 'e', 'x', 's', 'g', 'f', 'n', 'f', 'w', 'b', 'k', 't', 'e', 's', 's', 'w', 'w', 'p', 'w', 'o', 'e', 'n', 'a', 'g' } ]
        //];
        //List<List<IEnumerable<char>>> clusters = // разные
        //[
        //    [ new[] { 'e', 'b', 's', 'w', 't', 'a', 'f', 'c', 'b', 'g', 'e', 'c', 's', 's', 'w', 'w', 'p', 'w', 'o', 'p', 'k', 'n', 'm' } ],
        //    [ new[] { 'e', 'x', 'f', 'n', 't', 'n', 'f', 'c', 'b', 'n', 't', 'b', 's', 's', 'g', 'p', 'p', 'w', 'o', 'p', 'k', 'v', 'd' } ],
        //    [ new[] { 'e', 'x', 'f', 'n', 't', 'n', 'f', 'c', 'b', 'n', 't', 'b', 's', 's', 'p', 'g', 'p', 'w', 'o', 'p', 'n', 'y', 'd' } ],
        //    [ new[] { 'e', 'x', 'f', 'g', 't', 'n', 'f', 'c', 'b', 'n', 't', 'b', 's', 's', 'p', 'p', 'p', 'w', 'o', 'p', 'k', 'y', 'd' } ],
        //    [ new[] { 'e', 'x', 'y', 'n', 't', 'n', 'f', 'c', 'b', 'u', 't', 'b', 's', 's', 'g', 'g', 'p', 'w', 'o', 'p', 'n', 'v', 'd' } ]
        //];
        #endregion
        var clusters = new List<List<IEnumerable<int>>>();
        AddNewCluster(clusters);
        var cnt = 0;
        // phase 1
        foreach (var tr in transactions)
        {
            cnt++;
            double maxCost = 0;
            var bestChoice = 0;
            //var indexes = new Dictionary<double, int>();
            for (var i = 0; i < clusters.Count; i++)
            {
                var d1 = DateTime.Now;
                var da = DeltaAdd(clusters[i], tr, r);
                var d2 = DateTime.Now;
                var dt1dt1 = (d2 - d1).TotalMilliseconds;
                if (da > maxCost)
                {
                    maxCost = da;
                    bestChoice = i;
                }
                //var profit = Profit(clusters);
                //var profit = DeltaAdd(clusters[i], tr, r); // Infinity на 1-й итерации

                //indexes.TryAdd(profit, i);
            }
            //if (indexes.Count > 0)
            //{
            //var maxProfit = indexes.Select(x => x.Key).Max();
            //var idx = indexes[maxProfit];
            if (clusters[bestChoice].Count() == 0) AddNewCluster(clusters);
            clusters[bestChoice].Add(tr);
            //}
        }
        var trsCount1 = clusters.SelectMany(x => x).Count();
        var dt2 = DateTime.Now;
        var phase1 = (dt2 - dt1).TotalSeconds;
        var phase1Ms = (dt2 - dt1).TotalMilliseconds;
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
                var act = clusters.FirstOrDefault(x => x.Contains(tr)); // зафиксировать индекс кл-ра тр-и
                var actIdx = clusters.IndexOf(act);
                var dr = DeltaRemove(act, tr, r);
                for (var i = 0; i < clusters.Count; i++)
                {
                    if (clusters[i] == act)
                    {
                        //var a1 = new StringBuilder(); foreach (var cl in clusters[i]) { foreach (var c in cl) { a1.Append(c.ToString()); } a1.Append(' '); }
                        //var a2 = new StringBuilder(); foreach (var cl in act) { foreach (var c in cl) { a2.Append(c.ToString()); } a2.Append(' '); }
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
                    if (clusters[bestChoice].Count() == 0) AddNewCluster(clusters);

                    // перемещение тр-и
                    clusters[actIdx].Remove(tr);
                    clusters[bestChoice].Add(tr);
                    moved = true;
                }
            }
        }
        // TODO удалить пустые кластеры
        var trsCount2 = clusters.SelectMany(x => x).Count();
        return clusters;
    }

    //public List<List<IEnumerable<char>>> ClusterizeNoAddCluster(TCluster transactions, double r)
    //{
    //    //List<List<IEnumerable<char>>> clusters =
    //    //[
    //    //    [ new[] { 'a', 'b' }, new[] { 'a', 'b', 'c' } ], // кластер 1. [ 'a', 'b', 'c' ] - IReadOnlyArray, new[] { 'a', 'b' } - просто массив
    //    //    [ new[] { 'x', 's' }, new[] { 'y', 'w' } ] // кластер 2
    //    //];
    //    //List<List<IEnumerable<char>>> clusters = // похожие
    //    //[
    //    //    [ new[] { 'p', 'x', 's', 'n', 't', 'p', 'f', 'c', 'n', 'k', 'e', 'e', 's', 's', 'w', 'w', 'p', 'w', 'o', 'p', 'k', 's', 'u' } ],
    //    //    [ new[] { 'e', 'x', 's', 'y', 't', 'a', 'f', 'c', 'b', 'k', 'e', 'c', 's', 's', 'w', 'w', 'p', 'w', 'o', 'p', 'n', 'n', 'g' } ],
    //    //    [ new[] { 'e', 'b', 's', 'w', 't', 'l', 'f', 'c', 'b', 'n', 'e', 'c', 's', 's', 'w', 'w', 'p', 'w', 'o', 'p', 'n', 'n', 'm' } ],
    //    //    [ new[] { 'p', 'x', 'y', 'w', 't', 'p', 'f', 'c', 'n', 'n', 'e', 'e', 's', 's', 'w', 'w', 'p', 'w', 'o', 'p', 'k', 's', 'u' } ],
    //    //    [ new[] { 'e', 'x', 's', 'g', 'f', 'n', 'f', 'w', 'b', 'k', 't', 'e', 's', 's', 'w', 'w', 'p', 'w', 'o', 'e', 'n', 'a', 'g' } ]
    //    //];
    //    List<List<IEnumerable<char>>> clusters = // разные
    //    [
    //        [ new[] { 'e', 'b', 's', 'w', 't', 'a', 'f', 'c', 'b', 'g', 'e', 'c', 's', 's', 'w', 'w', 'p', 'w', 'o', 'p', 'k', 'n', 'm' } ],
    //        [ new[] { 'e', 'x', 'f', 'n', 't', 'n', 'f', 'c', 'b', 'n', 't', 'b', 's', 's', 'g', 'p', 'p', 'w', 'o', 'p', 'k', 'v', 'd' } ],
    //        [ new[] { 'e', 'x', 'f', 'n', 't', 'n', 'f', 'c', 'b', 'n', 't', 'b', 's', 's', 'p', 'g', 'p', 'w', 'o', 'p', 'n', 'y', 'd' } ],
    //        [ new[] { 'e', 'x', 'f', 'g', 't', 'n', 'f', 'c', 'b', 'n', 't', 'b', 's', 's', 'p', 'p', 'p', 'w', 'o', 'p', 'k', 'y', 'd' } ],
    //        [ new[] { 'e', 'x', 'y', 'n', 't', 'n', 'f', 'c', 'b', 'u', 't', 'b', 's', 's', 'g', 'g', 'p', 'w', 'o', 'p', 'n', 'v', 'd' } ]
    //    ];
    //    //var clusters = new List<List<IEnumerable<char>>>();
    //    foreach (var tr in transactions)
    //    {
    //        AddCluster(clusters, tr);
    //    }
    //    foreach (var tr in transactions)
    //    {
    //        var indexes = new Dictionary<double, int>();
    //        for (var i = 0; i < clusters.Count; i++)
    //        {
    //            clusters[i].Add(tr);
    //            var profit = Profit(clusters);

    //            indexes.TryAdd(profit, i);
    //            clusters[i].RemoveAt(clusters[i].Count() - 1);
    //        }
    //        //if (indexes.Count > 0)
    //        //{
    //        var maxProfit = indexes.Select(x => x.Key).Max();
    //        var idx = indexes[maxProfit];
    //        clusters[idx].Add(tr);
    //        //}
    //    }
    //    return clusters;
    //}

    private double Profit(List<List<IEnumerable<char>>> clusters)
    {
        double sum1 = 0;
        double sum2 = 0;
        foreach (var cl in clusters.Where(x => x.Count() > 0))
        {
            var trs = cl.SelectMany(x => x);
            var S = trs.Count();
            var W = Math.Pow(trs.GroupBy(x => x).Count(), r);
            sum1 += (double)(S / W) * cl.Count();
            sum2 += cl.Count();
        }
        var a1 = sum1 / sum2;
        return sum1/sum2;
    }
    private double DeltaAdd(List<IEnumerable<int>> C, TTransaction t, double r)
    {
        if (C.Count == 0) return t.Count() / Math.Pow(t.Count(), r);
        var trs = C.SelectMany(x => x);
        var C_S = trs.Count();
        var C_N = C.Count;
        var Snew = C_S + t.Count();
        var histogram = trs.GroupBy(x => x); //var histogram = trs.GroupBy(x => x).ToDictionary<int, double>();
        var histogram_ = new Dictionary<int, int>(); // <TOcc, TObject>
        foreach (var item in histogram.ToArray()) histogram_.Add(item.Key, item.Count()); //histogram.Select(x => histogram_.Add(x.Key, x.Count()));
        var C_W = histogram.Count();
        var Wnew = C_W;
        for (int i = 0; i < t.Count() - 1; i++)
        {
            if (!histogram_.ContainsKey(t.ElementAt(i))) // occ
            {
                Wnew += 1;
            }
        }
        var result = Snew * (C_N + 1) / Wnew.P(r) - C_S * C_N / C_W.P(r);
        return result;
    }

    //private double DeltaAdd(List<IEnumerable<int>> C, TTransaction t, double r)
    //{
    //    var C_ = new Cluster(C);
    //    if (C.Count == 0) return t.Count() / Math.Pow(t.Count(), r);
    //    var Snew = C_.S + t.Count();
    //    var Wnew = C_.W;
    //    for (int i = 0; i < t.Count() - 1; i++)
    //    {
    //        if (!C_.Histogram.ContainsKey(t.ElementAt(i))) // occ
    //        {
    //            Wnew += 1;
    //        }
    //    }
    //    var result = Snew * (C_.N + 1) / Wnew.P(r) - C_.S * C_.N / C_.W.P(r);
    //    return result;
    //}

    //private double DeltaAdd(Cluster C, TTransaction t, double r)
    //{
    //    if (C.Count == 0) return t.Count() / Math.Pow(t.Count(), r);
    //    var Snew = C.S + t.Count();
    //    var Wnew = C.W;
    //    for (int i = 0; i < t.Count() - 1; i++)
    //    {
    //        if (!C.Histogram.ContainsKey(t.ElementAt(i))) // occ
    //        {
    //            Wnew += 1;
    //        }
    //    }
    //    var result = Snew * (C.N + 1) / Wnew.P(r) - C.S * C.N / C.W.P(r);
    //    return result;
    //}

    //private double DeltaAdd(List<IEnumerable<int>> CL, TTransaction t, double r)
    //{
    //    C.self = CL;
    //    if (CL.Count == 0) return t.Count() / Math.Pow(t.Count(), r);
    //    var Snew = C.S + t.Count();
    //    var Wnew = C.W;
    //    for (int i = 0; i < t.Count() - 1; i++)
    //    {
    //        if (!C.Histogram.ContainsKey(t.ElementAt(i))) // occ
    //        {
    //            Wnew += 1;
    //        }
    //    }
    //    var result = Snew * (C.N + 1) / Wnew.P(r) - C.S * C.N / C.W.P(r);
    //    return result;
    //}

    private double DeltaRemove(List<IEnumerable<int>> C, TTransaction t, double r)
    {
        var trs = C.SelectMany(x => x);
        var C_S = trs.Count();
        var C_N = C.Count;
        var Snew = C_S - t.Count();
        var histogram = trs.GroupBy(x => x); //var histogram = trs.GroupBy(x => x).ToDictionary<int, double>();
        var histogram_ = new Dictionary<int, int>();
        foreach (var item in histogram.ToArray()) histogram_.Add(item.Key, item.Count()); //histogram.Select(x => histogram_.Add(x.Key, x.Count()));
        var C_W = histogram.Count();
        var Wnew = C_W;
        for (int i = 0; i < t.Count() - 1; i++)
        {
            if (histogram_[t.ElementAt(i)] == 1)
            {
                Wnew -= 1;
            }
        }
        var result = Snew * (C_N - 1) / Wnew.P(r) - C_S * C_N / C_W.P(r);
        return result;
    }

    private void AddNewCluster(List<List<IEnumerable<int>>> clusters)
    {
        var cluster = new List<IEnumerable<int>>();
        clusters.Add(cluster);
    }

    //private void AddCluster(List<List<IEnumerable<char>>> clusters, TTransaction transaction)
    //{
    //    var cluster = new List<IEnumerable<char>>();
    //    cluster.Add(transaction);
    //    clusters.Add(cluster);
    //}
}