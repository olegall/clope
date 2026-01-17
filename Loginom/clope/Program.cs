IEnumerable<char[]> transactions = [['a', 'b'], ['a', 'b', 'c'], ['a', 'c', 'd'], ['d', 'e'], ['d', 'e', 'f']];

IEnumerable<IEnumerable<char[]>> clustersOutput = 
[
    [ [ 'a', 'b' ], [ 'a', 'b', 'c' ], [ 'a', 'c', 'd' ] ], // кластер 1
    [ [ 'd', 'e' ], [ 'd', 'e', 'f' ] ] // кластер 2
];

var result = clusterize(transactions);
var debug = 0;
List<List<TTransaction>> clusterize<TTransaction>(IEnumerable<TTransaction> transactions) where TTransaction: IEnumerable<char>
{
    var clusters = new List<List<TTransaction>>();
    
    foreach (var tr in transactions)
    {
        if (clusters.Count == 0) {
            var cl = new List<TTransaction>();
            cl.Add(tr);
            clusters.Add(cl);
            continue;
        }
        for (var i = 0; i < clusters.Count; i++)
        {
            var trsInCluster = clusters.ElementAt(i); // кластер, набор транзакций
            if (trsInCluster.Any(x => x.Intersect(tr).Count() > 1)) // > k .All
            {
                trsInCluster.Add(tr);
            }
            else
            {
                var cl = new List<TTransaction>();
                cl.Add(tr);
                clusters.Add(cl);
            }
            // i = 0; continue;
        }
    }
    return clusters;
}

IEnumerable<char[]> transactions2 = [['a', 'c', 'd']];
var result2 = clusterize2(transactions2);
List<List<char[]>> clusterize2(IEnumerable<char[]> transactions)
{
    List<List<char[]>> clusters =
    [
        [ [ 'a', 'b' ], [ 'a', 'b', 'c' ] ], // кластер 1
        [ [ 'd', 'e' ], [ 'd', 'e', 'f' ] ] // кластер 2
    ];

    foreach (var tr in transactions)
    {
        var Hi = new Dictionary<double, int>();
        for (var i = 0; i < clusters.Count; i++)
        {
            var H = GetH(clusters[i], tr);
            Hi.Add(H, i);
        }
        var maxH = Hi.Select(x => x.Key).Max();
        var i_ = Hi[maxH];
        clusters[i_].Add(tr);
    }
    return clusters;
}

double GetH(IList<char[]> cluster, char[] transactionToCluster)
{
    var trs = cluster.SelectMany(x => x).Concat(transactionToCluster);
    var grouped = trs.GroupBy(x => x);
    var W = grouped.Count();
    var S = trs.Count();
    return S/W;
}

//List<TCluster> clusterize<TCluster>(TCluster transactions) where TCluster : IEnumerable<IEnumerable<char>>, new()
//{
//    var clusters = new List<TCluster>();
//    foreach (var tr in transactions)
//    {
//        var a1 = new TCluster();
//        if (clusters.Count == 0) clusters.Add();
//    }

//    return clusters;
//}