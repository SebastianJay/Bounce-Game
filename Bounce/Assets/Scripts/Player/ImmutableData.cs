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
		//InitItem (ItemType.Item1, "Accessories/sample_glasses", "Glasses", "A nondescript pair of glasses", 0f, 0f, 0f);
		InitItem (ItemType.CyborgEye, "Accessories/Glasses", "Cyborg Eye", "A nondescript pair of glasses", 0f, 0f, 0f);
		InitItem (ItemType.YellowSki, "Accessories/Glasses1", "Yellow Ski Goggles", "Life is more extreme with these on.", 0f, 0f, 0f);
		InitItem (ItemType.DarkShades, "Accessories/Glasses2", "Dark Shades", "Sadly, you won't look as cool as the special agents that wear these.", 0f, 0f, 0f);
		InitItem (ItemType.LabGoggles, "Accessories/Glasses3", "Lab Goggles", "Safety first!", 0f, 0f, 0f);
		InitItem (ItemType.Groucho, "Accessories/Glasses4", "Groucho Glasses", "Become a master of disguise with these glasses!", 0f, 0f, 0f);
		InitItem (ItemType.Goggles, "Accessories/Glasses5", "Goggles", "These don't actually protect your eyes. They just make you look cooler than everybody else.", 0f, 0f, 0f);
		InitItem (ItemType.BlueShades, "Accessories/Glasses6", "Blue Shades", "You'll go blind if you stare at the sun too long without these.", 0f, 0f, 0f);
		InitItem (ItemType.Visor, "Accessories/Glasses7", "Visor", "Essential for maintaining proper control of your optic beams.", 0f, 0f, 0f);
		InitItem (ItemType.Oculus, "Accessories/Glasses8", "Oculus Rift", "3D virtual reality has never looked this amazing. This isn't even the final form.", 0f, 0f, 0f);
		InitItem (ItemType.Aviator, "Accessories/Glasses9", "Aviator Shades", "Classic and timeless shades that blend together form and function.", 0f, 0f, 0f);
		InitItem (ItemType.GreenShutter, "Accessories/Glasses10", "Green Shutter Shades", "You'll make a spectacle of yourself if you go out wearing these.  Seriously, the tan lines are not worth wearing these.", 0f, 0f, 0f);
		InitItem (ItemType.Heart, "Accessories/Glasses11", "Heart Shades", "Contrary to popular belief, pink can be a manly color.", 0f, 0f, 0f);
		InitItem (ItemType.Future, "Accessories/Glasses12", "Future Goggles", "These may not look impressive, but they somehow protect you from the vortices and light speeds that you will encounter while time traveling.", 0f, 0f, 0f);
		InitItem (ItemType.Monocole, "Accessories/Glasses13", "Monocle and Stache", "A proper gentleman never leaves the house without an elegant monocle and a neatly trimmed beard.", 0f, 0f, 0f);
		InitItem (ItemType.Tiny, "Accessories/Glasses14", "Tiny Spectacles", "A nondescript pair of glasses", 0f, 0f, 0f);
		InitItem (ItemType.DarkFuture, "Accessories/Glasses15", "Dark Future Glasses", "A nondescript pair of glasses", 0f, 0f, 0f);
		InitItem (ItemType.CrazyEyes, "Accessories/Glasses16", "Crazy Eyes", "A nondescript pair of glasses", 0f, 0f, 0f);
		InitItem (ItemType.EightBit1, "Accessories/Glasses17", "8-bit Glasses #1", "A nondescript pair of glasses", 0f, 0f, 0f);
		InitItem (ItemType.EightBit2, "Accessories/Glasses18", "8-bit Glasses #2", "A nondescript pair of glasses", 0f, 0f, 0f);
		InitItem (ItemType.PurpleVisor, "Accessories/Glasses19", "Purple Visor", "A nondescript pair of glasses", 0f, 0f, 0f);
		InitItem (ItemType.BlueGreenSki, "Accessories/Glasses20", "Blue-Green Ski Goggles", "A nondescript pair of glasses", 0f, 0f, 0f);
		InitItem (ItemType.Movie3D, "Accessories/Glasses21", "3D Glasses", "A nondescript pair of glasses", 0f, 0f, 0f);
		InitItem (ItemType.ClownNose, "Accessories/clown nose skin", "Clown Nose", "I'm serious: don't even think about doing any kind of funny business.", 0f, 0f, 0f);
		InitItem (ItemType.AngryBob, "Accessories/angry bob skin", "Angry Bob!", "You wouldn't like me when I'm angry.", 0f, 0f, 0f);

		init = true;
	}
}
