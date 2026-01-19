namespace clope;

internal class Clope<TTransaction> where TTransaction : IEnumerable<char>
{
    public List<List<TTransaction>> Clusterize(List<TTransaction> transactions, double r)
    {
        var clusters = new List<List<TTransaction>>();
        AddNewCluster(clusters, transactions[0]);
        transactions.RemoveAt(0);

        foreach (var tr in transactions)
        {
            var indexes = new Dictionary<double, int>();
            for (var i = 0; i < clusters.Count; i++)
            {
                var G = GetGradient(clusters[i], tr);

                if (G < r)
                {
                    if (i == clusters.Count - 1)
                    {
                        AddNewCluster(clusters, tr);
                        break;
                    }
                    continue;
                }
                else
                {
                    indexes.TryAdd(G, i);
                    break;
                }
            }

            if (indexes.Count > 0)
            {
                var maxG = indexes.Select(x => x.Key).Max();
                var idx = indexes[maxG];
                clusters[idx].Add(tr);
            }
        }
        return clusters;
    }

    private void AddNewCluster(List<List<TTransaction>> clusters, TTransaction transaction)
    {
        var cluster = new List<TTransaction>();
        cluster.Add(transaction);
        clusters.Add(cluster);
    }

    private double GetGradient(IEnumerable<TTransaction> cluster, TTransaction transaction) // не учитываются вхождения a:3,b:2,c:2,d:1
    {
        var transactions = cluster.SelectMany(x => x).Concat(transaction);
        var W = transactions.GroupBy(x => x).Count();
        var S = transactions.Count();
        return (double)S / (W * W);
    }
}

