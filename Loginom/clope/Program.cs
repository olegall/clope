// See https://aka.ms/new-console-template for more information
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;

Console.WriteLine("Hello, World!");

var a = new[] { 'a', 'b' };

//var a1 = new List<IEnumerable<char>> { new[] { 'a', 'b' }, new[] { 'a', 'b', 'c' }, new[] { 'a', 'c', 'd' }, new[] { 'd', 'e' }, new[] { 'd', 'e', 'f' } };
var transactions = new[] {
    new[] { 'a', 'b' }, new[] { 'a', 'b', 'c' }, new[] { 'a', 'c', 'd' }, new[] { 'd', 'e' }, new[] { 'd', 'e', 'f' } // транзакции
};

IEnumerable<IEnumerable<char>> transactions_short = [
    [ 'a', 'b' ], [ 'a', 'b', 'c' ] // транзакции
];

var output = new[] {
    new[] { new[] { 'a', 'b' }, new[] { 'a', 'b', 'c' }, new[] { 'a', 'c', 'd' } }, // кластер 1
    new[] { new[] { 'd', 'e' }, new[] { 'd', 'e', 'f' } } // кластер 2
};

var result = clusterize(transactions);
var debug = 0;
List<List<TTransaction>> clusterize<TTransaction>(IEnumerable<TTransaction> transactions) where TTransaction: IEnumerable<char>
{
    var clusters = new List<List<TTransaction>>();
    foreach (var tr in transactions)
    {
        if (clusters.Count == 0) {
            var cluster = new List<TTransaction>(); // List<List
            cluster.Add(tr);
            clusters.Add(cluster);
            continue;
        }

        for (var i = 0; i < clusters.Count-1; i++)
        {
            if (clusters.ElementAt(i).Any(x => x.Intersect(tr).Count() > 1)) // clusters.ElementAt(i) - транзакции
                clusters.ElementAt(i).Add(tr);
            else 
                clusters.ElementAt(i+1).Add(tr);
        }
    }
    return clusters;
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