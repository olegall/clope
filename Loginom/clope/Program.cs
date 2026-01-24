using clope;

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
var result2 = new Clope<char[], List<char[]>>().Clusterize(transactions2.ToList(), 0.3);
var result21 = new Clope2<char[], List<char[]>>().Clusterize(transactions2.ToList(), 2.6);
var break_ = 0;

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