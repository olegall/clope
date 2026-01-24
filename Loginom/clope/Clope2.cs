namespace clope;

internal class Clope2<TTransaction, TCluster> where TTransaction : IEnumerable<char> where TCluster : List<TTransaction>, new()
{
    private const double r = 2.6;
    public List<TCluster> Clusterize(TCluster transactions, double r)
    {
        var clusters = new List<TCluster>();
        foreach (var tr in transactions)
        {
            var indexes = new Dictionary<double, int>();
            AddNewCluster(clusters); // для текущей транзакции по всем кластерам, нужен, при попадании туда тр-и Profit мб максимальным
            for (var i = 0; i < clusters.Count; i++)
            {
                clusters[i].Add(tr);

                var profit = Profit(clusters);
                indexes.TryAdd(profit, i);
                //if (clusters.Count() > 1)
                //    clusters[i].RemoveAt(clusters[i].Count()-1);
            }
            //if (indexes.Count > 0)
            //{
                var maxProfit = indexes.Select(x => x.Key).Max();
                var idx = indexes[maxProfit];
                clusters[idx].Add(tr);
            //}
            //if (clusters.ElementAt(clusters.Count()-1).Count() == 0)
            if (clusters.Count() > 1)
                clusters.RemoveAt(clusters.Count() - 1);
        }
        return clusters;
    }

    private double Profit(List<TCluster> clusters)
    {
        double sum1 = 0;
        double sum2 = 0;
        foreach (var cl in clusters)
        {
            var trs = cl.SelectMany(x => x);
            var W = Math.Pow(trs.GroupBy(x => x).Count(), r);
            var S = trs.Count();
            sum1 += (double)(S / W) * cl.Count();
            sum2 += cl.Count();
        }
        return sum1/sum2;
    }

    private void AddNewCluster(List<TCluster> clusters)
    {
        var cluster = new TCluster();
        clusters.Add(cluster);
    }
}