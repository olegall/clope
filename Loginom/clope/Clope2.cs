namespace clope;

internal class Clope2<TTransaction, TCluster> where TTransaction : IEnumerable<char> where TCluster : List<TTransaction>, new()
{
    private const double r = 2.6;
    public List<List<IEnumerable<char>>> Clusterize(TCluster transactions, double r)
    {
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

        List<List<IEnumerable<char>>> clusters =
        [
            [ new[] { 'a', 'b' }, new[] { 'a', 'b', 'c' } ], // кластер 1. [ 'a', 'b', 'c' ] - IReadOnlyArray, new[] { 'a', 'b' } - просто массив
            [ new[] { 'x', 's' }, new[] { 'y', 'w' } ] // кластер 2
        ];
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
            //if (clusters.Last().Count() == 0) // if (clusters.Count() > 1)
            //    clusters.RemoveAt(clusters.Count() - 1);
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

    private void AddNewCluster(List<List<IEnumerable<char>>> clusters)
    {
        var cluster = new List<IEnumerable<char>>();
        clusters.Add(cluster);
    }
}