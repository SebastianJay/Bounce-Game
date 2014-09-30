using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class XmlSerialzer : MonoBehaviour {

	public static int currentSaveFile = 0;

	public static void Save(playerData p)
	{
		string savePath = "BounceSaveData" + currentSaveFile;
		var serializer = new XmlSerializer(typeof(playerData));
		using(var stream = new FileStream(savePath, FileMode.Create))
		{
			serializer.Serialize(stream,p);
		}
	}
	
	public static playerData Load()
	{
		string savePath = "BounceSaveData" + currentSaveFile;
		var serializer = new XmlSerializer(typeof(playerData));
		try{
		using(var stream = new FileStream(savePath, FileMode.Open))
		{
			return serializer.Deserialize(stream) as playerData;
		}
		}catch(FileNotFoundException e)
		{
			return null;
		}
	}
}

[XmlRoot("PlayerData")]
public class playerData
{
	[XmlAttribute("lastCheckpoint")]
	public int lastCheckpoint;

	[XmlAttribute("lastLevel")]
	public int lastLevel;

	[XmlArray("previousCheckpoints")]
	[XmlArrayItem("Entry")]
	public List<Entry> previousCheckpoints = new List<Entry>();
	//Each element of the list corresponds to a level which has a list of checkpoints visited

	[XmlArray("Inventory")]
	[XmlArrayItem("int")]
	public List<int> Inventory = new List<int>();
}
[XmlRoot("Entry")]
public class Entry
{
	[XmlAttribute("key")]
	public int key;

	[XmlArray("value")]
	[XmlArrayItem("int")]
	public List<int> value;

	public Entry(int k, List<int> v)
	{
		key = k;
		value = v;
	}
	public Entry()
	{
	}
}


