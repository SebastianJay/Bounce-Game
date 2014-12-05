using UnityEngine;
using System.Collections.Generic;

public class PlayerDataManager : MonoBehaviour {

	// Use this for initialization
	public int lastCheckpoint = 0;
	public int lastLevel = 0;
	public bool debugNoLoad = false;
	public bool debugInitRespawn = true;
	public bool debugInitCamera = true;
	public Dictionary<int,List<int>> previousCheckpoints = new Dictionary<int,List<int>>();
	public Inventory inventory = new Inventory();
	public HashSet<string> gameConstants = new HashSet<string>();

	public static bool loadedLevel = false;
	public static int initialLevel = 0;

	public playerData myData;

	void Start () {
		if (!debugNoLoad)
			LoadCurrentSave ();
		if (debugInitCamera)
		{
			GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
			CameraFollowConfig cfc = new CameraFollowConfig();
			cfc.minXAndY = new Vector2(-1000f, -1000f);
			cfc.maxXAndY = new Vector2(1000f, 1000f);
			cfc.isLocked = false;
			cfc.lockedOrthoSize = 7f;
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
			initialLevel = lastLevel;
			Application.LoadLevel(myData.lastLevel);
			loadedLevel = true;
			
			
		}
	}

	void OnLevelWasLoaded(int level)
	{
		myData = XmlSerialzer.Load ();
		if (myData != null && loadedLevel && level == initialLevel)
		{
			
			lastCheckpoint = myData.lastCheckpoint;
			
			Vector3 myPos = Vector3.zero;
			Checkpoint.posCheckTable.TryGetValue(lastCheckpoint,out myPos);

			transform.position = myPos;
			
			List<Entry> entries = myData.previousCheckpoints;
			previousCheckpoints.Clear();
			foreach (Entry e in entries)
			{
				previousCheckpoints[e.key] = e.value;
			}
			
			inventory.Load(myData.inventory);
		}else if(myData != null && loadedLevel)
		{
			lastCheckpoint = myData.lastCheckpoint;
			List<Entry> entries = myData.previousCheckpoints;
			previousCheckpoints.Clear();
			foreach (Entry e in entries)
			{
				previousCheckpoints[e.key] = e.value;
			}
			inventory.Load(myData.inventory);
		}


		if (LevelTeleporter.teleported) {
			LevelTeleporter.teleported = false;
			transform.position = LevelTeleporter.teleporterTargetTable[LevelTeleporter.teleportTarget];
		}
	}

	public void saveCurrent()
	{
		Debug.Log ("Saving");
		List<Entry> entries = new List<Entry>();
		foreach (int key in previousCheckpoints.Keys)
		{
			entries.Add (new Entry(key,previousCheckpoints[key]));
		}
		myData = new playerData ();
		myData.previousCheckpoints = entries;
		foreach(Entry e in myData.previousCheckpoints)
		{
			foreach(int i in e.value)
			{
				Debug.Log (i);
			}

		}
		myData.inventory = inventory.ToList();
		myData.lastCheckpoint = lastCheckpoint;
		myData.lastLevel = lastLevel;
		XmlSerialzer.Save (myData);
	}
}
