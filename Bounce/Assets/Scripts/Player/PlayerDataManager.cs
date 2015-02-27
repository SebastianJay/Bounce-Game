using UnityEngine;
using System.Collections.Generic;

public class PlayerDataManager : MonoBehaviour {

	public bool debugNoLoad = false;
	public bool debugInitRespawn = true;
	public bool debugInitCamera = true;

	public static int lastCheckpoint = 0;
	public static int lastLevel = 0;
	public static Inventory inventory = new Inventory();
	//public static Dictionary<int,List<int>> previousCheckpoints = new Dictionary<int,List<int>>();
	public static HashSet<int> previousCheckpoints = new HashSet<int>();

	public static bool loadedLevel = false;
	public static int checkpointID = 0;
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

	public void LoadCurrentSave()
	{
		myData = XmlSerialzer.Load ();
		if(myData != null && !loadedLevel)
		{
			lastLevel = myData.lastLevel;
			Application.LoadLevel(myData.lastLevel);
			//initialLevel = lastLevel;
			loadedLevel = true;
		}
	}

	void OnLevelWasLoaded(int level)
	{
		//GameObject player = GameObject.FindGameObjectWithTag("Player");
		//Debug.Log (player.transform.position);

		if (/*myData != null && */loadedLevel)
		{
			ImmutableData.CheckpointData cData = ImmutableData.GetCheckpointData()[lastCheckpoint];

			GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
			cam.GetComponent<CameraFollow>().LoadConfig(cData.camConfig);

			transform.position = cData.location;
		}
		/*
		myData = XmlSerialzer.Load ();
		if (myData != null && loadedLevel && level == initialLevel)
		{
			
			lastCheckpoint = myData.lastCheckpoint;
			
			Vector3 myPos = Vector3.zero;
			Checkpoint.posCheckTable.TryGetValue(lastCheckpoint,out myPos);

			transform.position = myPos;
			
			List<PlayerDataEntry> entries = myData.previousCheckpoints;
			previousCheckpoints.Clear();
			foreach (PlayerDataEntry e in entries)
			{
				previousCheckpoints[e.key] = e.value;
			}
			
			inventory.Load(myData.inventory);
		}else if(myData != null && loadedLevel)
		{
			lastCheckpoint = myData.lastCheckpoint;
			List<PlayerDataEntry> entries = myData.previousCheckpoints;
			previousCheckpoints.Clear();
			foreach (PlayerDataEntry e in entries)
			{
				previousCheckpoints[e.key] = e.value;
			}
			inventory.Load(myData.inventory);
		}


		if (LevelTeleporter.teleported) {
			LevelTeleporter.teleported = false;
			transform.position = LevelTeleporter.teleporterTargetTable[LevelTeleporter.teleportTarget];
		}
		*/
	}

	public void SaveCurrent()
	{
		Debug.Log ("Saving");
		List<int> entries = new List<int>(previousCheckpoints);
		//foreach (int key in previousCheckpoints)
		//{
		//	entries.Add (key);
		//}
		myData = new PlayerData();
		myData.previousCheckpoints = entries;
		myData.inventory = inventory.ToList();
		myData.lastCheckpoint = lastCheckpoint;
		myData.lastLevel = lastLevel;
		XmlSerialzer.Save (myData);
	}
}
