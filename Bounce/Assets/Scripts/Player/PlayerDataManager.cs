using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerDataManager : MonoBehaviour {

	public bool debugNoLoad = false;
	public bool debugInitRespawn = true;
	public bool debugInitCamera = true;

	public static int lastCheckpoint = 0;
	public static int lastLevel = 0;
	public static ItemType itemEquipped = ItemType.None;
	public static Inventory inventory = new Inventory();
	public static HashSet<int> previousCheckpoints = new HashSet<int>();

	public static long secondsSinceSave = 0;
	public static DateTime timeSinceSave = DateTime.Now;
	public static long numberDeaths = 0;
	public static int numberUniqueItems = 0;

	public static bool loadedLevel = false;		//if true, player teleported or loaded using menu

	//public static int checkpointID = 0;
	//public static int initialLevel = 0;

	public static PlayerData myData;

	void Start () {
		if (!debugNoLoad)
			LoadCurrentSave ();
		if (debugInitCamera && !loadedLevel)
		{
			GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
			CameraFollowConfig cfc = new CameraFollowConfig();
			cfc.minXAndY = new Vector2(-1000f, -1000f);
			cfc.maxXAndY = new Vector2(1000f, 1000f);
			cfc.isLocked = false;
			cfc.orthoSize = 7f;
			cfc.position = new Vector3(this.transform.position.x, this.transform.position.y, -10f);
			cam.GetComponent<CameraFollow>().LoadConfig(cfc);
		}
		if (debugInitRespawn)
		{
			Death.respawn = new Vector2(transform.position.x, transform.position.y);
			Death.camConfig = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>().GetConfig();
		}
	}

	void OnLevelWasLoaded(int level)
	{
		if (/*myData != null && */loadedLevel)
		{
			ImmutableData.CheckpointData cData = ImmutableData.GetCheckpointData()[lastCheckpoint];

			GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
			cam.GetComponent<CameraFollow>().LoadConfig(cData.camConfig);

			transform.position = cData.location;

		}
		if (itemEquipped != ItemType.None) {
			GetComponent<AccessoryManager>().SetAccessory(itemEquipped);
		}
		lastLevel = level;
	}

	public static void LoadCurrentSave()
	{
		myData = BounceXmlSerializer.Load ();
		if(myData != null && !loadedLevel)
		{
			lastLevel = myData.lastLevel;
			lastCheckpoint = myData.lastCheckpoint;
			inventory.Load(myData.inventory);
			DialogueConstantParser.constantSet.Clear();
			DialogueConstantParser.constantSet.UnionWith(myData.constants);
			previousCheckpoints.Clear ();
			previousCheckpoints.UnionWith(myData.previousCheckpoints);
			itemEquipped = myData.itemEquipped;
			numberDeaths = myData.numberDeaths;
			timeSinceSave = DateTime.Now;
			secondsSinceSave = myData.numberSeconds;
			numberUniqueItems = inventory.GetNumUniqueItems();

			Application.LoadLevel(myData.lastLevel);
			loadedLevel = true;
		}
	}

	public static void SaveCurrent()
	{
		//Debug.Log ("Saving");
		List<int> entries = new List<int>(previousCheckpoints);
		myData = new PlayerData();
		myData.previousCheckpoints = entries;
		myData.inventory = inventory.ToList();
		myData.lastCheckpoint = lastCheckpoint;
		myData.lastLevel = lastLevel;
		myData.constants = DialogueConstantParser.constantSet;
		myData.itemEquipped = itemEquipped;
		myData.numberSeconds = secondsSinceSave + (long)((DateTime.Now - timeSinceSave).TotalSeconds);
		myData.numberDeaths = numberDeaths;
		myData.numberCollectibles = inventory.GetNumUniqueItems();
		BounceXmlSerializer.Save (myData);
		timeSinceSave = DateTime.Now;
	}

	public static void DownloadCurrent()
	{
		List<int> entries = new List<int>(previousCheckpoints);
		myData = new PlayerData();
		myData.previousCheckpoints = entries;
		myData.inventory = inventory.ToList();
		myData.lastCheckpoint = lastCheckpoint;
		myData.lastLevel = lastLevel;
		myData.constants = DialogueConstantParser.constantSet;
		myData.itemEquipped = itemEquipped;
		myData.numberSeconds = secondsSinceSave + (long)((DateTime.Now - timeSinceSave).TotalSeconds);
		myData.numberDeaths = numberDeaths;
		myData.numberCollectibles = inventory.GetNumUniqueItems();
		BounceXmlSerializer.SaveAndDownload (myData);
		timeSinceSave = DateTime.Now;
	}

	public static void UploadCurrent(PlayerData myData)
	{
		if(myData != null)
		{
			lastLevel = myData.lastLevel;
			lastCheckpoint = myData.lastCheckpoint;
			inventory.Load(myData.inventory);
			DialogueConstantParser.constantSet.Clear();
			DialogueConstantParser.constantSet.UnionWith(myData.constants);
			previousCheckpoints.Clear ();
			previousCheckpoints.UnionWith(myData.previousCheckpoints);
			itemEquipped = myData.itemEquipped;
			numberDeaths = myData.numberDeaths;
			timeSinceSave = DateTime.Now;
			secondsSinceSave = myData.numberSeconds;
			numberUniqueItems = inventory.GetNumUniqueItems();
			
			Application.LoadLevel(myData.lastLevel);
			loadedLevel = true;
		} else {
			Debug.LogError("Bad data file.");
		}
	}
}
