using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Linq;

public static class BounceXmlSerializer {

	public static int currentSaveFile = -1;
	public const string saveDirectory = "Save_Data/";
	public const string savePrefix = "BounceSaveData";
	public const string saveSuffix = ".sav";

	private static XmlReader reader;

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

	//doesn't use the serialization APIs, but just looks at attributes at the top level
	public static List<BounceFileMetaData> RetrieveMetaData()
	{
		List<string> fullFileList = Directory.GetFiles (saveDirectory, savePrefix + "*" + saveSuffix, 
		                                                SearchOption.TopDirectoryOnly).ToList();
		List<BounceFileMetaData> dataList = new List<BounceFileMetaData> ();
		foreach (string filename in fullFileList)
		{
			reader = XmlReader.Create(filename);
			XmlDocument doc = new XmlDocument();
			doc.Load(reader);
			XmlNodeList nodeLst = doc.ChildNodes;
			foreach (XmlNode node in nodeLst)
			{
				if (node.Name == "PlayerData") {
					BounceFileMetaData metadata = new BounceFileMetaData();
					if (node.Attributes["numberDeaths"] != null)
						metadata.numberDeaths = long.Parse(node.Attributes["numberDeaths"].Value);
					if (node.Attributes["numberCollectibles"] != null)
						metadata.numberCollectibles = int.Parse(node.Attributes["numberCollectibles"].Value);
					if (node.Attributes["playTime"] != null)
						metadata.numberSeconds = long.Parse(node.Attributes["playTime"].Value);
					if (node.Attributes["lastCheckpoint"] != null)
						metadata.lastCheckpoint = int.Parse(node.Attributes["lastCheckpoint"].Value);
					dataList.Add(metadata);
				}
			}
			reader.Close();
		}
		return dataList;
	}
}

[XmlRoot("PlayerData")]
public class PlayerData
{
	[XmlAttribute("lastCheckpoint")]
	public int lastCheckpoint = 0;

	[XmlAttribute("lastLevel")]
	public int lastLevel = 0;

	[XmlAttribute("numberDeaths")]
	public long numberDeaths = 0;

	[XmlAttribute("numberCollectibles")]
	public int numberCollectibles = 0;

	[XmlAttribute("playTime")]
	public long numberSeconds = 0;

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

public struct BounceFileMetaData
{
	public int lastCheckpoint;
	public long numberDeaths;
	public int numberCollectibles;
	public long numberSeconds;
}