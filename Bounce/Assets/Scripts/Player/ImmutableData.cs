using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Where all the hard-coded data about certain parts of the game/menu which don't need to be serialized are stored.
/// </summary>
public static class ImmutableData {

	public struct CheckpointData {
		public int id;							//unique, even across levels
		public string name;						//display name in-game
		public Vector3 location;				//where player is placed
		public CameraFollowConfig camConfig;	//how camera should be configured
	}

	public struct ItemData {
		public ItemType id;						//unique
		public string name;						//display name in-game
		public Sprite image;					//what item looks like
		public string description;				//more detail for menu
		public Vector2 localPosition;			//where item is placed on character
		public Quaternion localRotation;			//how item should be rotated
	}

	//checkpoint locations in each level
	public static Dictionary<int, CheckpointData> checkpointMapping;

	//item config
	public static Dictionary<ItemType, ItemData> itemMapping;

	//specifying which checkpoints belong to which levels
	public static Dictionary<string, List<int> > levelMapping;

	private static bool init=false;
	static ImmutableData() {
		Init ();
	}

	//for the camera released state
	private static void InitCheck(int id, string name, float xloc, float yloc, 
	                              float xcam, float ycam,
	                              float xcammin, float xcammax, float ycammin, float ycammax, float orthosize) {
		CheckpointData data;
		data.id = id;
		data.name = name;
		data.location = new Vector3 (xloc, yloc, 0f);
		
		CameraFollowConfig camConfig = new CameraFollowConfig();
		camConfig.isLocked = false;
		camConfig.minXAndY = new Vector2 (xcammin, ycammin);
		camConfig.maxXAndY = new Vector2 (xcammax, ycammax);
		camConfig.position = new Vector3(xcam, ycam, CameraFollow.camZCoordinate);
		camConfig.orthoSize = orthosize;
		
		data.camConfig = camConfig;
		checkpointMapping[id] = data;
	}

	//for the convenience camera locked state
	private static void InitCheck(int id, string name, float xloc, float yloc, 
	                              float xcam, float ycam, float orthosize) {
		CheckpointData data = new CheckpointData();
		data.id = id;
		data.name = name;
		data.location = new Vector3 (xloc, yloc, 0f);

		CameraFollowConfig camConfig = new CameraFollowConfig();
		camConfig.isLocked = true;
		camConfig.position = new Vector3(xcam, ycam, CameraFollow.camZCoordinate);
		camConfig.lockedPosition = new Vector3(xcam, ycam, CameraFollow.camZCoordinate);
		camConfig.orthoSize = orthosize;

		data.camConfig = camConfig;
		checkpointMapping[id] = data;
	}
	
	private static void InitItem(ItemType type, string path, string name, string description,
	                             float xpos, float ypos, float zrot) {
		ItemData data = new ItemData();
		data.id = type;
		data.image = Resources.Load<Sprite>(path);
		data.name = name;
		data.description = description;
		data.localPosition = new Vector3 (xpos, ypos, 0f);
		//data.localRotation = Quaternion.LookRotation (new Vector3 (0f, 0f, zrot));
		itemMapping[type] = data;
	}

	private static void InitLevel(string name, params int[] checkID)
	{
		if (!levelMapping.ContainsKey (name))
			levelMapping [name] = new List<int> ();
		levelMapping[name].AddRange (checkID);
	}
	
	public static Dictionary<int, CheckpointData > GetCheckpointData() { return checkpointMapping; }
	public static Dictionary<ItemType, ItemData > GetItemData() { return itemMapping; }
	public static Dictionary<string, List<int> > GetLevelData() { return levelMapping; }

	public static void Init() {
		if (init)
			return;
		checkpointMapping = new Dictionary<int, CheckpointData> ();
		itemMapping = new Dictionary<ItemType, ItemData> ();
		levelMapping = new Dictionary<string, List<int> > ();

		//Hub checkpoints
		InitCheck (1, "The Big Ship", 150f, 13.5f, 150f, 13.5f, -1000f, 1000f, -1000f, 1000f, 11f);

		//City checkpoints
		InitCheck (100, "The Chimney", -20f, 51f, -12f, 51f, -15f, 300f, -5f, 50f, 7f);
		InitCheck (101, "Picnic Spot", 235f, 1.4f, 235f, 5.5f, 217f, 248f, 5.5f, 6.5f, 7f);	
		InitCheck (102, "Break Room", 460f, 1.6f, 462f, 6.5f, 407f, 462f, 6.5f, 12f, 8f);	
		InitCheck (103, "Tower Top", 487f, 151f, 488f, 154f, 488f, 488f, 7f, 200f, 8f);	

		//Jungle checkpoints
		InitCheck (200, "Treetop Village", -20f, 49f, -20f, 50f, -35f, -20f, 28f, 50f, 8f);
		InitCheck (201, "Treebottom Town", 28f, -74f, 27f, -71f, 12f, 35f, -71f, -70f, 7f);	

		//level mappings
		InitLevel ("Pier", 1);
		InitLevel ("City", 100, 101, 102, 103);
		InitLevel ("Jungle", 200, 201);

		//Items!
		InitItem (ItemType.Item1, "Accessories/sample_glasses", "Glasses", "A nondescript pair of glasses", 0f, 0f, 0f);
		InitItem (ItemType.Item2, "Accessories/Glasses2", "Glasses", "A nondescript pair of glasses", 0f, 0f, 0f);
		InitItem (ItemType.Item3, "Accessories/Glasses3", "Glasses", "A nondescript pair of glasses", 0f, 0f, 0f);
		InitItem (ItemType.Item4, "Accessories/sample_glasses", "Glasses", "A nondescript pair of glasses", 0f, 0f, 0f);
		InitItem (ItemType.Item5, "Accessories/sample_glasses", "Glasses", "A nondescript pair of glasses", 0f, 0f, 0f);
		InitItem (ItemType.Item6, "Accessories/sample_glasses", "Glasses", "A nondescript pair of glasses", 0f, 0f, 0f);
		InitItem (ItemType.Item7, "Accessories/sample_glasses", "Glasses", "A nondescript pair of glasses", 0f, 0f, 0f);

		init = true;
	}
}
