IEnumerable<char[]> transactions = [['a', 'b'], ['a', 'b', 'c'], ['a', 'c', 'd'], ['d', 'e'], ['d', 'e', 'f']];
//IEnumerable<char[]> transactions = [['a', 'b'], ['a', 'b', 'c'], ['a', 'c', 'd'], ['v', 'w'], ['x', 'y', 'z']];

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

//IEnumerable<char[]> transactions2 = [['a', 'b'], ['a', 'b', 'c'], ['a', 'c', 'd'], ['d', 'e'], ['d', 'e', 'f']];
//IEnumerable<char[]> transactions2 = [['a', 'b'], ['a', 'b', 'c'], ['d', 'e'], ['d', 'e', 'f']]; // рабит, 2 кластера H:1.5 G:0.3
IEnumerable<char[]> transactions2 = [['a', 'c', 'd'], ['a', 'd', 'e'], ['x'], ['y', 'z']]; // рабит, 2 кластера H:1.5 G:0.3-неточно
//IEnumerable<char[]> transactions2 = [['y', 'z'], ['a', 'c', 'd']];
//IEnumerable<char[]> transactions2 = [['a', 'c', 'd'], ['y', 'z']];
var result2 = clusterize2(transactions2.ToList(), 0.3);
List<List<char[]>> clusterize2(List<char[]> transactions, double r)
{
    var clusters = new List<List<char[]>>();
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

void AddNewCluster(List<List<char[]>> clusters, char[] transaction)
{
    var cl = new List<char[]>();
    cl.Add(transaction);
    clusters.Add(cl);
}

double GetGradient(IEnumerable<char[]> cluster, char[] transaction) // не учитываются вхождения a:3,b:2,c:2,d:1
{
    var transactions = cluster.SelectMany(x => x).Concat(transaction);
    var W = transactions.GroupBy(x => x).Count();
    var S = transactions.Count();
    return (double)S/(W*W);
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