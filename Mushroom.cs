namespace clope;

class Mushroom
{
	static readonly Dictionary<int, Dictionary<char, int>> mapping = new Dictionary<int, Dictionary<char, int>>
		{
			{1, new Dictionary<char, int> {
				{'p', 1},
				{'e', 2}
			} },
			{2, new Dictionary<char, int> {
				{'b', 3},
				{'c', 4},
				{'x', 5},
				{'f', 6},
				{'k', 7},
				{'s', 8}
			} },
			{3, new Dictionary<char, int> {
				{'f', 9},
				{'g', 10},
				{'y', 11},
				{'s', 12}
			} },
			{4, new Dictionary<char, int> {
				{'n', 13},
				{'b', 14},
				{'c', 15},
				{'g', 16},
				{'r', 17},
				{'p', 18},
				{'u', 19},
				{'e', 20},
				{'w', 21},
				{'y', 22}
			} },
			{5, new Dictionary<char, int> {
				{'t', 23},
				{'f', 24}
			} },
			{6, new Dictionary<char, int> {
				{'a', 25},
				{'l', 26},
				{'c', 27},
				{'y', 28},
				{'f', 29},
				{'m', 30},
				{'n', 31},
				{'p', 32},
				{'s', 33}
			} },
			{7, new Dictionary<char, int> {
				{'a', 34},
				{'d', 35},
				{'f', 36},
				{'n', 37}
			} },
			{8, new Dictionary<char, int> {
				{'c', 38},
				{'w', 39},
				{'d', 40}
			} },
			{9, new Dictionary<char, int> {
				{'b', 41},
				{'n', 42}
			} },
			{10, new Dictionary<char, int> {
				{'k', 43},
				{'n', 44},
				{'b', 45},
				{'h', 46},
				{'g', 47},
				{'r', 48},
				{'o', 49},
				{'p', 50},
				{'u', 51},
				{'e', 52},
				{'w', 53},
				{'y', 54},
			} },
			{11, new Dictionary<char, int> {
				{'e', 55},
				{'t', 56}
			} },
			{12, new Dictionary<char, int> {
				{'b', 57},
				{'c', 58},
				{'u', 59},
				{'e', 60},
				{'z', 61},
				{'r', 62},
	               //{'?', }
	           } },
			{13, new Dictionary<char, int> {
				{'f', 63},
				{'y', 64},
				{'k', 65},
				{'s', 66}
			} },
			{14, new Dictionary<char, int> {
				{'f', 67},
				{'y', 68},
				{'k', 69},
				{'s', 70}
			} },
			{15, new Dictionary<char, int> {
				{'n', 71},
				{'b', 72},
				{'c', 73},
				{'g', 74},
				{'o', 75},
				{'p', 76},
				{'e', 77},
				{'w', 78},
				{'y', 79}
			} },
			{16, new Dictionary<char, int> {
				{'n', 80},
				{'b', 81},
				{'c', 82},
				{'g', 83},
				{'o', 84},
				{'p', 85},
				{'e', 86},
				{'w', 87},
				{'y', 88}
			} },
			{17, new Dictionary<char, int> {
				{'p', 89},
				{'u', 90}
			} },
			{18, new Dictionary<char, int> {
				{'n', 91},
				{'o', 92},
				{'w', 93},
				{'y', 94}
			} },
			{19, new Dictionary<char, int> {
				{'n', 95},
				{'o', 96},
				{'t', 97}
			} },
			{20, new Dictionary<char, int> {
				{'c', 98},
				{'e', 99},
				{'f', 100},
				{'l', 101},
				{'n', 102},
				{'p', 103},
				{'s', 104},
				{'z', 105}
			} },
			{21, new Dictionary<char, int> {
				{'k', 106},
				{'n', 107},
				{'b', 108},
				{'h', 109},
				{'r', 110},
				{'o', 111},
				{'u', 112},
				{'w', 113},
				{'y', 114}
			} },
			{22, new Dictionary<char, int> {
				{'a', 115},
				{'c', 116},
				{'n', 117},
				{'s', 118},
				{'v', 119},
				{'y', 120}
			} },
			{23, new Dictionary<char, int> {
				{'g', 121},
				{'l', 122},
				{'m', 123},
				{'p', 124},
				{'u', 125},
				{'w', 126},
				{'d', 127}
			} }
	};

	public static IEnumerable<int[]> Normalize(string inputFilePath)
	{
		var sr = new StreamReader(inputFilePath);
		var result_ = new List<int[]>();
		while (!sr.EndOfStream)
		{
            var line = sr.ReadLine().Replace(" ", "");
			//line = line.Substring(2, line.Length - 2);
			var attributes = line.Split(',');
			var result = new List<int>();
			for (int i = 0; i < attributes.Length; i++)
			{
				if (attributes[i] != "?")
					result.Add(mapping[i + 1][attributes[i][0]]);
			}
            result_.Add(result.ToArray());
        }
		return result_;
	}
}
