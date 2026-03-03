using clope;

var data = MushroomDataSet.Normalize("data\\agaricus-lepiota.data");
Clope.Clusterize(data.ToList());