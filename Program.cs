using clope;

const string srcDataFilepath = "D:\\Работа\\Компании\\Loginom\\clope\\data\\agaricus-lepiota.data";
var trs = Mushroom.Normalize(srcDataFilepath);
//var result21 = new Clope2<int[], List<int[]>>().Clusterize(trs/*.Skip(1000).Take(1000)*/.ToList(), 2.6);
var resultArch = Clope.Clusterize(trs/*.Skip(1000).Take(1000)*/.ToList(), 2.6);
var break_ = 0;