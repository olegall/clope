using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clope;

public interface ICLOPEData
{
    Transaction ReadNextTransaction();
    void WriteTransaction(Transaction transaction);
    void ToBegin();
    bool IsEnd();
}

public class Transaction
{
    public List<int> Parameters { get { return parameters; } }
    public int ClusterNumber { get { return clusterNumber; } set { clusterNumber = value; } }

    public Transaction(List<int> parameters, int clusterNumber)
    {
        this.parameters = parameters;
        this.clusterNumber = clusterNumber;
    }

    List<int> parameters;
    int clusterNumber;
}
class CLOPEData : ICLOPEData
{
	const int rowLength = 80;
	const int beginPosition = -1;
	FileStream fileStream;
	int position = beginPosition;

	public CLOPEData(string inputFilePath, string outputFilePath)
	{
		StreamReader sr = new StreamReader(inputFilePath);
		StreamWriter sw = new StreamWriter(outputFilePath);
		while (!sr.EndOfStream)
			sw.WriteLine((sr.ReadLine() + ",").PadRight(rowLength - 2, ' '));
		sr.Close();
		sw.Close();

		fileStream = new FileStream(outputFilePath, FileMode.Open);
	}

	public Transaction ReadNextTransaction()
	{
		byte[] buffer = new byte[rowLength];
		fileStream.Read(buffer, 0, rowLength);
		position++;
		List<int> parameters = new List<int>();
		Encoding.ASCII.GetString(buffer, 0, rowLength - 2).Split(',').ToList().ForEach(new Action<string>((e) =>
		{
			int value;
			int.TryParse(e.Trim(), out value);
			parameters.Add(value);
		}));
		int clusterNumber = parameters[parameters.Count - 1];
		parameters.RemoveAt(parameters.Count - 1);
		return new Transaction(parameters, clusterNumber);
	}

	public void WriteTransaction(Transaction transaction)
	{
		string result = "";
		transaction.Parameters.ForEach(new Action<int>((e) =>
		{
			result += e + ",";
		}));
		result += transaction.ClusterNumber;
		result = result.PadRight(rowLength - 2) + "\r\n";
		fileStream.Seek(position * rowLength, SeekOrigin.Begin);
		fileStream.Write(Encoding.ASCII.GetBytes(result), 0, result.Length);
	}

	public void ToBegin()
	{
		position = beginPosition;
		fileStream.Seek(0, SeekOrigin.Begin);
	}

	public bool IsEnd()
	{
		return fileStream.Position == fileStream.Length;
	}

	public void Close()
	{
		fileStream.Close();
	}
}
