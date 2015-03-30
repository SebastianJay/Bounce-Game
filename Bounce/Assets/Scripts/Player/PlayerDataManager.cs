﻿using UnityEngine;
using System.Collections.Generic;

public class PlayerDataManager : MonoBehaviour {

	public bool debugNoLoad = false;
	public bool debugInitRespawn = true;
	public bool debugInitCamera = true;

	public static int lastCheckpoint = 0;
	public static int lastLevel = 0;
	public static ItemType itemEquipped = ItemType.None;
	public static Inventory inventory = new Inventory();
	public static HashSet<int> previousCheckpoints = new HashSet<int>();

	public static bool loadedLevel = false;		//if true, player teleported or loaded using menu
	//public static int checkpointID = 0;
	//public static int initialLevel = 0;

	public PlayerData myData;

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
	}

	public void LoadCurrentSave()
	{
		myData = XmlSerialzer.Load ();
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

			Application.LoadLevel(myData.lastLevel);
			loadedLevel = true;
		}
	}

	public void SaveCurrent()
	{
		Debug.Log ("Saving");
		List<int> entries = new List<int>(previousCheckpoints);
		myData = new PlayerData();
		myData.previousCheckpoints = entries;
		myData.inventory = inventory.ToList();
		myData.lastCheckpoint = lastCheckpoint;
		myData.lastLevel = lastLevel;
		myData.constants = DialogueConstantParser.constantSet;
		myData.itemEquipped = itemEquipped;
		XmlSerialzer.Save (myData);
	}
}
