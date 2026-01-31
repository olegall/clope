namespace clope;

internal class Clope2<TTransaction, TCluster> where TTransaction : IEnumerable<char> where TCluster : List<TTransaction>, new()
{
    private const double r = 2.6;
    public List<List<IEnumerable<char>>> Clusterize(TCluster transactions, double r)
    {
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
        List<List<IEnumerable<char>>> clusters = // разные
        [
            [ new[] { 'e', 'b', 's', 'w', 't', 'a', 'f', 'c', 'b', 'g', 'e', 'c', 's', 's', 'w', 'w', 'p', 'w', 'o', 'p', 'k', 'n', 'm' } ],
            [ new[] { 'e', 'x', 'f', 'n', 't', 'n', 'f', 'c', 'b', 'n', 't', 'b', 's', 's', 'g', 'p', 'p', 'w', 'o', 'p', 'k', 'v', 'd' } ],
            [ new[] { 'e', 'x', 'f', 'n', 't', 'n', 'f', 'c', 'b', 'n', 't', 'b', 's', 's', 'p', 'g', 'p', 'w', 'o', 'p', 'n', 'y', 'd' } ],
            [ new[] { 'e', 'x', 'f', 'g', 't', 'n', 'f', 'c', 'b', 'n', 't', 'b', 's', 's', 'p', 'p', 'p', 'w', 'o', 'p', 'k', 'y', 'd' } ],
            [ new[] { 'e', 'x', 'y', 'n', 't', 'n', 'f', 'c', 'b', 'u', 't', 'b', 's', 's', 'g', 'g', 'p', 'w', 'o', 'p', 'n', 'v', 'd' } ]
        ];
        #endregion
        //var clusters = new List<List<IEnumerable<char>>>();
        foreach (var tr in transactions)
        {
            var indexes = new Dictionary<double, int>();
            //AddNewCluster(clusters); // для текущей транзакции по всем кластерам, нужен, при попадании туда тр-и Profit мб максимальным
            for (var i = 0; i < clusters.Count; i++)
            {
                //var clusters_ = clusters.Select(x => { var cl = new TCluster(); cl = (TCluster)x.ToList(); return cl; }).ToList();
                //clusters_[i].Add(tr);
                //var profit = Profit(clusters_);

                //var profit = Profit(clusters);
                var profit = DeltaAdd(clusters[i], tr, r);

                indexes.TryAdd(profit, i);
            }
            //if (indexes.Count > 0)
            //{
                var maxProfit = indexes.Select(x => x.Key).Max();
                var idx = indexes[maxProfit];
                clusters[idx].Add(tr);
            //}
            //if (clusters.Last().Count() == 0) // if (clusters.Count() > 1)
            //    clusters.RemoveAt(clusters.Count() - 1);
        }
        return clusters;
    }

    public List<List<IEnumerable<char>>> ClusterizeNoAddCluster(TCluster transactions, double r)
    {
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
        List<List<IEnumerable<char>>> clusters = // разные
        [
            [ new[] { 'e', 'b', 's', 'w', 't', 'a', 'f', 'c', 'b', 'g', 'e', 'c', 's', 's', 'w', 'w', 'p', 'w', 'o', 'p', 'k', 'n', 'm' } ],
            [ new[] { 'e', 'x', 'f', 'n', 't', 'n', 'f', 'c', 'b', 'n', 't', 'b', 's', 's', 'g', 'p', 'p', 'w', 'o', 'p', 'k', 'v', 'd' } ],
            [ new[] { 'e', 'x', 'f', 'n', 't', 'n', 'f', 'c', 'b', 'n', 't', 'b', 's', 's', 'p', 'g', 'p', 'w', 'o', 'p', 'n', 'y', 'd' } ],
            [ new[] { 'e', 'x', 'f', 'g', 't', 'n', 'f', 'c', 'b', 'n', 't', 'b', 's', 's', 'p', 'p', 'p', 'w', 'o', 'p', 'k', 'y', 'd' } ],
            [ new[] { 'e', 'x', 'y', 'n', 't', 'n', 'f', 'c', 'b', 'u', 't', 'b', 's', 's', 'g', 'g', 'p', 'w', 'o', 'p', 'n', 'v', 'd' } ]
        ];
        //var clusters = new List<List<IEnumerable<char>>>();
        foreach (var tr in transactions)
        {
            AddCluster(clusters, tr);
        }
        foreach (var tr in transactions)
        {
            var indexes = new Dictionary<double, int>();
            for (var i = 0; i < clusters.Count; i++)
            {
                clusters[i].Add(tr);
                var profit = Profit(clusters);

                indexes.TryAdd(profit, i);
                clusters[i].RemoveAt(clusters[i].Count() - 1);
            }
            //if (indexes.Count > 0)
            //{
            var maxProfit = indexes.Select(x => x.Key).Max();
            var idx = indexes[maxProfit];
            clusters[idx].Add(tr);
            //}
        }
        return clusters;
    }

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
    private double DeltaAdd(List<IEnumerable<char>> C, TTransaction t, double r)
    {
        var trs = C.SelectMany(x => x);
        var C_S = trs.Count();
        var C_N = C.Count();
        var Snew = C_S + t.Count();
        var Occ = trs.GroupBy(x => x);
        var Wnew = Occ.Count();
        var keys = Occ.Select(x => x.Key);
        for (int i = 0; i < t.Count() - 1; i++)
        {
            if (i < keys.Count() && keys.Contains(t.ElementAt(i)) && Occ.ElementAt(i).Count() == 0)
            {
                Snew += 1;
            }
        }
        var result = Snew * (C_N + 1) / Math.Pow(Wnew, r) - C_S * C_N / Math.Pow(Wnew, r);
        return result;
    }

    private void AddNewCluster(List<List<IEnumerable<char>>> clusters)
    {
        var cluster = new List<IEnumerable<char>>();
        clusters.Add(cluster);
    }

    private void AddCluster(List<List<IEnumerable<char>>> clusters, TTransaction transaction)
    {
        var cluster = new List<IEnumerable<char>>();
        cluster.Add(transaction);
        clusters.Add(cluster);
    }
}