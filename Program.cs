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
IEnumerable<char[]> transactions2 = [['a', 'c', 'd'], ['a', 'd', 'e'], ['u', 'v', 'w'], ['x', 'y', 'z']]; // рабит, 3 кластера
//IEnumerable<char[]> transactions2 = [['y', 'z'], ['a', 'c', 'd']];
//IEnumerable<char[]> transactions2 = [['a', 'c', 'd'], ['y', 'z']];
IEnumerable<char[]> transactions3 = 
[
    // похожие
    //['e','x','y','y','t','a','f','c','b','n','e','c','s','s','w','w','p','w','o','p','k','n','g'],
    //['e','b','s','w','t','a','f','c','b','g','e','c','s','s','w','w','p','w','o','p','k','n','m'],
    //['e','b','y','w','t','l','f','c','b','n','e','c','s','s','w','w','p','w','o','p','n','s','m'],
    //['p','x','y','w','t','p','f','c','n','p','e','e','s','s','w','w','p','w','o','p','k','v','g'],
    //['e','b','s','y','t','a','f','c','b','g','e','c','s','s','w','w','p','w','o','p','k','s','m'],
    
    // разные, взяты случайно на всему файлу. тест: wwpw
    ['e','x','f','n','f','n','f','w','b','n','t','e','s','f','w','w','p','w','o','e','k','a','g'],
    ['p','x','s','n','t','p','f','c','n','n','e','e','s','s','w','w','p','w','o','p','k','s','g'],
    ['e','b','y','w','t','a','f','c','b','w','e','c','s','s','w','w','p','w','o','p','k','s','m'],
    ['e','x','f','g','t','n','f','c','b','u','t','b','s','s','g','g','p','w','o','p','n','y','d'],
    ['p','k','y','y','f','n','f','w','n','w','e','c','y','y','y','y','p','y','o','e','w','c','l'],
    ['z','z','z','z','z','z','z','z','z','z','z','z','z','z','z','z','z','z','z','z','z','z','z']
    //[e,x,y,y,t,l,f,c,b,g,e,c,s,s,w,w,p,w,o,p,n,n,g],
    //[e,x,y,y,t,a,f,c,b,n,e,c,s,s,w,w,p,w,o,p,k,s,m],
    //[e,b,s,y,t,a,f,c,b,w,e,c,s,s,w,w,p,w,o,p,n,s,g],
    //[p,x,y,w,t,p,f,c,n,k,e,e,s,s,w,w,p,w,o,p,n,v,u],
    //[e,x,f,n,f,n,f,w,b,n,t,e,s,f,w,w,p,w,o,e,k,a,g]
]; // рабит, 2 кластера H:1.5 G:0.3-неточно
IEnumerable<char[]> fromClusters = // разные
[
    ['e','x','f','n','f','n','f','w','b','n','t','e','s','f','w','w','p','w','o','e','k','a','g'], // из transaction3
    ['p','x','s','n','t','p','f','c','n','n','e','e','s','s','w','w','p','w','o','p','k','s','g'],
    ['e','b','y','w','t','a','f','c','b','w','e','c','s','s','w','w','p','w','o','p','k','s','m'],
    ['e','x','f','g','t','n','f','c','b','u','t','b','s','s','g','g','p','w','o','p','n','y','d'],
    ['p','k','y','y','f','n','f','w','n','w','e','c','y','y','y','y','p','y','o','e','w','c','l'],
    ['z','z','z','z','z','z','z','z','z','z','z','z','z','z','z','z','z','z','z','z','z','z','z'],

    ['e','b','s','w','t','a','f','c','b','g','e','c','s','s','w','w','p','w','o','p','k','n','m'],
    ['e','x','f','n','t','n','f','c','b','n','t','b','s','s','g','p','p','w','o','p','k','v','d'],
    ['e','x','f','n','t','n','f','c','b','n','t','b','s','s','p','g','p','w','o','p','n','y','d'],
    ['e','x','f','g','t','n','f','c','b','n','t','b','s','s','p','p','p','w','o','p','k','y','d'],
    ['e','x','y','n','t','n','f','c','b','u','t','b','s','s','g','g','p','w','o','p','n','v','d'],
    ['z','z','z','z','z','z','z','z','z','z','z','z','z','z','z','z','z','z','z','z','z','z','z']
];
IEnumerable<char[]> transactions4 = [
    ['a', 'a', 'a'], ['a', 'a', 'a'], ['a', 'a', 'a'], 
    ['b', 'b', 'b'], ['b', 'b', 'b'], ['b', 'b', 'b'],
    ['c', 'c', 'c'], ['c', 'c', 'c'], ['c', 'c', 'c']
];

const string srcDataFilepath = "D:\\Работа\\Компании\\Loginom\\clope\\data\\agaricus-lepiota.data";
var trs = Mushroom.Normalize(srcDataFilepath);
//.Skip(1000).Take(100) // перетасуются кластеры на фазе 2
//var result21 = new Clope2<int[], List<int[]>>().Clusterize(trs/*.Skip(1000).Take(1000)*/.ToList(), 2.6);
var resultArch = new ClopeArch().Clusterize(trs/*.Skip(1000).Take(1000)*/.ToList(), 2.6);
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