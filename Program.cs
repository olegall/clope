using clope;

var data = Mushroom.Normalize("data\\agaricus-lepiota.data");
//var result21 = new Clope2<int[], List<int[]>>().Clusterize(trs/*.Skip(1000).Take(1000)*/.ToList(), 2.6);
var resultArch = Clope.Clusterize(data.ToList());
var break_ = 0;