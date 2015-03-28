using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class XmlSerialzer : MonoBehaviour {

	public static int currentSaveFile = 0;
	public const string saveDirectory = "Save_Data/";
	public const string savePrefix = "BounceSaveData";
	public const string saveSuffix = ".sav";

	public static void Save(PlayerData p)
	{
		if (!Directory.Exists(saveDirectory))
			Directory.CreateDirectory(saveDirectory);
		string savePath = saveDirectory + savePrefix + currentSaveFile + saveSuffix;
		var serializer = new XmlSerializer(typeof(PlayerData));
		using(var stream = new FileStream(savePath, FileMode.Create))
		{
			serializer.Serialize(stream,p);
		}
	}
	
	public static PlayerData Load()
	{
		if (!Directory.Exists(saveDirectory))
			return null;
		string savePath = saveDirectory + savePrefix + currentSaveFile + saveSuffix;
		var serializer = new XmlSerializer(typeof(PlayerData));
		try{
		using(var stream = new FileStream(savePath, FileMode.Open))
		{
			return serializer.Deserialize(stream) as PlayerData;
		}
		}catch(FileNotFoundException e)
		{
			return null;
		}
	}
}

[XmlRoot("PlayerData")]
public class PlayerData
{
	[XmlAttribute("lastCheckpoint")]
	public int lastCheckpoint = 0;

	[XmlAttribute("lastLevel")]
	public int lastLevel = 0;

	[XmlArray("previousCheckpoints")]
	[XmlArrayItem("int")]
	public List<int> previousCheckpoints = new List<int>();

	[XmlArray("inventory")]
	[XmlArrayItem("int")]
	public List<int> inventory = new List<int>();

	//[XmlArray("gameConsts")]
	[XmlElement("string")]
	public HashSet<string> constants = new HashSet<string>();

	[XmlAttribute("itemEquipped")]
	public ItemType itemEquipped;
}

/*
[XmlRoot("Entry")]
public class PlayerDataEntry
{
	[XmlAttribute("key")]
	public int key;

	[XmlArray("value")]
	[XmlArrayItem("int")]
	public List<int> value;

	public PlayerDataEntry(int k, List<int> v)
	{
		key = k;
		value = v;
	}
	public PlayerDataEntry()
	{
	}
}
*/

