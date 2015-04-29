using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class MainMenu : MonoBehaviour
{
	public enum MenuTab
	{
		Save,
		Inventory,
		Map
	};

	public const float EPSILON = 1e-11f;
	public const float MOUSE_EPSILON = 1e-5f;
	public MenuTab currentTab = MenuTab.Save;
	public bool showMenu = false;	//fix later
	
	public float menuWidth = 500;
	public float menuHeight = 510;
	public float tabButtonWidth = 150;
	public float tabButtonHeight = 50;
	public float scrollViewWidth = 480;
	public float scrollViewHeight = 400;

	public AudioClip openMenuSound;
	public AudioClip closeMenuSound;
	public AudioClip loadSound;
	public AudioClip saveSound;
	public AudioClip teleportSound;
	public AudioClip itemSound;
	public AudioClip switchPanelSound;

	public float openMenuVolume = 1f;
	public float closeMenuVolume = 1f;
	public float loadVolume = 1f;
	public float saveVolume = 1f;
	public float teleportVolume = 1f;
	public float itemVolume = 1f;
	public float switchPanelVolume = 1f;

	private AudioSource openMenuSrc;
	private AudioSource closeMenuSrc;
	private AudioSource loadSrc;
	private AudioSource saveSrc;
	private AudioSource teleportSrc;
	private AudioSource itemSrc;
	private AudioSource switchPanelSrc;

	Vector2 scrollPosition = Vector2.zero;
	Vector2 scrollPosition2 = Vector2.zero;
	Vector2 scrollPositionI = Vector2.zero;
	
	private bool menuWasOpen = false;		//represents the first "tick" of active menu
	private GameObject player;
	private GameObject screenFadeObj;
	List<BounceFileMetaData> saveFileList = new List<BounceFileMetaData>();

	public Texture2D cursorTexture;
	public CursorMode cursorMode = CursorMode.Auto;
	public Vector2 hotSpot = Vector2.zero;
	public float cursorShowTime = 3.0f;
	private float cursorShowTimer = 0.0f;
	private Vector3 cursorLastPosition;

	// the right way to do this is to define a skin
	// however, since we have different size buttons we'll do it the bad way (many styles)
	//	(this version of GUI is deprecated anyway, so..) 
	public GUIStyle panelStyle;
	public GUIStyle labelStyle;
	//public GUIStyle scrollbarStyle;
	public GUIStyle button50x50Style;
	public GUIStyle button120x50Style;
	public GUIStyle button150x50Style;
	public GUIStyle button240x50Style;

	//and for some reason scrollbars don't have a well defined style..
	public GUISkin scrollbarSkin;

	// Use this for initialization
	void Start ()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		screenFadeObj = GameObject.FindGameObjectWithTag("ScreenFader");
		//init sound
		if (openMenuSound != null){
			openMenuSrc = gameObject.AddComponent<AudioSource>();
			openMenuSrc.clip = openMenuSound;
			openMenuSrc.volume = openMenuVolume;
		}
		if (closeMenuSound != null){
			closeMenuSrc = gameObject.AddComponent<AudioSource>();
			closeMenuSrc.clip = closeMenuSound;
			closeMenuSrc.volume = closeMenuVolume;
		}
		if (loadSound != null){
			loadSrc = gameObject.AddComponent<AudioSource>();
			loadSrc.clip = loadSound;
			loadSrc.volume = loadVolume;
		}
		if (saveSound != null){
			saveSrc = gameObject.AddComponent<AudioSource>();
			saveSrc.clip = saveSound;
			saveSrc.volume = saveVolume;
		}
		if (teleportSound != null){
			teleportSrc = gameObject.AddComponent<AudioSource>();
			teleportSrc.clip = teleportSound;
			teleportSrc.volume = teleportVolume;
		}
		if (itemSound != null){
			itemSrc = gameObject.AddComponent<AudioSource>();
			itemSrc.clip = itemSound;
			itemSrc.volume = itemVolume;
		}
		if (switchPanelSound != null){
			switchPanelSrc = gameObject.AddComponent<AudioSource>();
			switchPanelSrc.clip = switchPanelSound;
			switchPanelSrc.volume = switchPanelVolume;
		}
		saveFileList = BounceXmlSerializer.RetrieveMetaData();
	}
	
	void OnMouseEnter() {
		Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
	}

	void OnMouseExit() {
		Cursor.SetCursor(null, Vector2.zero, cursorMode);
	}

	// Update is called once per frame
	void Update ()
	{
		if (player == null)
			player = GameObject.FindGameObjectWithTag ("Player");
		if (Input.GetButtonDown("Menu") && 
		    !DialogueConstantParser.eventLock && 
		    ((player.GetComponent<PlayerBallControl>() == null ||
			 (!player.GetComponent<PlayerBallControl>().playerLock)) &&
		    (player.GetComponent<PlayerBodyControl>() == null || 
		 	 (!player.GetComponent<PlayerBodyControl>().playerLock)) &&
		    (screenFadeObj == null || !screenFadeObj.GetComponent<ScreenFading>().IsTransitioning()))) {
		
			if (!showMenu && openMenuSrc != null)
				openMenuSrc.Play();
			if (showMenu && closeMenuSrc != null)
				closeMenuSrc.Play();
			showMenu = !showMenu;
		}
		if (!showMenu) {
			if ((Input.mousePosition - cursorLastPosition).magnitude < MOUSE_EPSILON) {
				cursorShowTimer += Time.deltaTime;
				if (cursorShowTimer >= cursorShowTime) {
					Screen.showCursor = false;
				}
			} else {
				cursorShowTimer = 0f;
				Screen.showCursor = true;
			}
			cursorLastPosition = Input.mousePosition;
		} else {
			Screen.showCursor = true;
		}

		if (Input.GetKeyDown(KeyCode.A))
		{
			BounceXmlSerializer.RetrieveMetaData();
		}
	}

	void OnGUI ()
	{
		if (showMenu) {
			bool fadingOut = screenFadeObj != null && screenFadeObj.GetComponent<ScreenFading>().IsTransitioning();

			//Freeze the game if the menu is active
			Time.timeScale = EPSILON;

			//Main menu frame
			GUI.Box (new Rect (Screen.width / 2 - menuWidth / 2, Screen.height / 2 - menuHeight / 2, menuWidth, menuHeight), "", panelStyle);

			//Buttons for the three tabs
			if (GUI.Button (new Rect (Screen.width / 2 - menuWidth / 2 + 10, 
			                          Screen.height / 2 - menuHeight / 2 + 10, 
			                          tabButtonWidth, tabButtonHeight), "Saves", button150x50Style)) {
				if (currentTab != MenuTab.Save && switchPanelSrc != null)
					switchPanelSrc.Play ();
				currentTab = MenuTab.Save;
			}
			if (GUI.Button (new Rect (Screen.width / 2 - tabButtonWidth / 2, 
			                          Screen.height / 2 - menuHeight / 2 + 10, 
			                          tabButtonWidth, tabButtonHeight), "Collection", button150x50Style)) {
				if (currentTab != MenuTab.Inventory && switchPanelSrc != null)
					switchPanelSrc.Play ();
				currentTab = MenuTab.Inventory;
			}
			if (GUI.Button (new Rect (Screen.width / 2 + menuWidth / 2 - 10 - tabButtonWidth, 
			                          Screen.height / 2 - menuHeight / 2 + 10, 
			                          tabButtonWidth, tabButtonHeight), "Map", button150x50Style)) {
				if (currentTab != MenuTab.Map && switchPanelSrc != null)
					switchPanelSrc.Play ();
				currentTab = MenuTab.Map;
			}

			//The following layout of the menu is dependent on which tab is open
			//Save tab layout - scrollable list of save files
			//from which you can save, load, or create a new save
			if (currentTab == MenuTab.Save) {

				GUI.skin = scrollbarSkin;
				scrollPosition = GUI.BeginScrollView (new Rect (Screen.width / 2 - menuWidth / 2 + 10, 
				                                                Screen.height / 2 - menuHeight / 2 + 15 + tabButtonHeight, 
				                                                scrollViewWidth, scrollViewHeight), scrollPosition, 
				                                      new Rect (0, 0, scrollViewWidth - 20, saveFileList.Count*50+10));
				GUI.skin = null;

				// A button for creating a new save file
				if(GUI.Button (new Rect(0,10,240,tabButtonHeight),"Save to New File", button240x50Style) && !fadingOut)
				{
					if (saveSrc != null)
						saveSrc.Play();
					BounceXmlSerializer.currentSaveFile = saveFileList.Count;
					PlayerDataManager.SaveCurrent();
					saveFileList = BounceXmlSerializer.RetrieveMetaData();
					GameObject noteObj = GameObject.FindGameObjectWithTag("NoteManager");
					noteObj.GetComponent<NotificationManager>().PushMessage("Saved game.");
				}
				if (BounceXmlSerializer.currentSaveFile >= 0 && saveFileList.Count > 0) {
					// A button for creating a new save file
					if(GUI.Button (new Rect(240,10,240,tabButtonHeight),"Overwrite current data (" + BounceXmlSerializer.currentSaveFile + ")", button240x50Style) && !fadingOut)
					{
						if (saveSrc != null)
							saveSrc.Play();
						PlayerDataManager.SaveCurrent();
						saveFileList = BounceXmlSerializer.RetrieveMetaData();
						GameObject noteObj = GameObject.FindGameObjectWithTag("NoteManager");
						noteObj.GetComponent<NotificationManager>().PushMessage("Saved game.");
					}
				}
				
				// A list of all our save files
				for(int i = 1; i < saveFileList.Count+1; i++)
				{
					//string s = saveFileList[i-1];
					int saveFileIndex = i-1;
					//GUI.Label (new Rect (10, i*60+10, scrollViewWidth-10, 50),"File "+saveFileIndex, labelStyle);
					BounceFileMetaData metadata = saveFileList[saveFileIndex];
					long minutes = (metadata.numberSeconds/60)%60;
					long hours = metadata.numberSeconds/3600;
					GUI.Label (new Rect (10, i*60+10, 110, 20), ImmutableData.GetCheckpointData()[metadata.lastCheckpoint].name, labelStyle);
					GUI.Label (new Rect (10, i*60+30, 110, 20), string.Format("Playtime: {0:D2}:{1:D2}", hours, minutes), labelStyle);
					GUI.Label (new Rect (120, i*60+10, 110, 20), string.Format("Collectibles: {0}", metadata.numberCollectibles), labelStyle);
					GUI.Label (new Rect (120, i*60+30, 110, 20), string.Format("Deaths: {0}", metadata.numberDeaths), labelStyle);
					if(GUI.Button (new Rect(240,i*60+10,120,50),"Overwrite", button120x50Style) && !fadingOut)
					{
						if (saveSrc != null)
							saveSrc.Play();
						BounceXmlSerializer.currentSaveFile = metadata.index;
						PlayerDataManager.SaveCurrent();
						saveFileList = BounceXmlSerializer.RetrieveMetaData();
						GameObject noteObj = GameObject.FindGameObjectWithTag("NoteManager");
						noteObj.GetComponent<NotificationManager>().PushMessage("Saved game.");
					}
					if(GUI.Button (new Rect(360,i*60+10,120,50),"Load", button120x50Style) && !fadingOut)
					{
						if (loadSrc != null)
							loadSrc.Play();
						loadDataIndex = metadata.index;
						if (screenFadeObj != null) {
							screenFadeObj.GetComponent<ScreenFading>().fadeSpeed /= EPSILON;
							screenFadeObj.GetComponent<ScreenFading>().Transition(LoadTransition, true);
						} else
							LoadTransition();

					}
				}						
				GUI.EndScrollView ();
			}

			//Inventory tab layout - grid of items
			//hovering over an item yields its name and description
			if (currentTab == MenuTab.Inventory) {

				//GUI.skin = scrollbarSkin;
				//scrollPositionI = GUI.BeginScrollView (new Rect (Screen.width / 2 - menuWidth / 2 + 10, Screen.height / 2 - menuHeight / 2 + 15 + tabButtonHeight, scrollViewWidth, scrollViewHeight), 
				//                                       scrollPositionI, new Rect (0, 0, scrollViewWidth - 20, scrollViewHeight * 4));
				//GUI.skin = null;
				GUI.BeginGroup(new Rect(Screen.width/2 - menuWidth/2 + 10, Screen.height/2-menuHeight/2 + 15 + tabButtonHeight, scrollViewWidth, scrollViewHeight));

				//Inventory inventory = player.GetComponent<PlayerDataManager>().inventory;
				/*
				Inventory inventory = PlayerDataManager.inventory;
				int count = inventory.items.Length;
				float gridHeight = (float)count/inventoryGridX;
				gridHeight = Mathf.Ceil(gridHeight*100);
				string[] itemNames = new string[inventory.ToList().Count];
				for(int i=0; i < inventory.ToList().Count; i++){
					itemNames[i] = inventory.items[i].ToString();
				}
				*/
				Dictionary<ItemType, ImmutableData.ItemData>.Enumerator iter = ImmutableData.GetItemData().GetEnumerator();			
				if (iter.MoveNext()) {
					bool iterdone = false;
					//Todo: adjust bounds based on number of items & adjust sizes of buttons dynamically based on menu panel size
					for (int i = 0; i < 5; i++) {
						for (int j = 0; j < 7; j++)
						{
							Rect buttonBoundingBox = new Rect(10 + (j * 60), 120 + (i * 60), 50, 50);
							if (PlayerDataManager.inventory.HasItem(iter.Current.Key)) {
								string nameStr = "";
								string infoStr = "";
								if (buttonBoundingBox.Contains(Event.current.mousePosition)) {
									//Debug.Log ("Mouse in button (" + i + ", " + j + ")");
									nameStr = iter.Current.Value.name;
									infoStr = iter.Current.Value.description;
								}
								GUI.Label(new Rect(10, 10, 420, 30), nameStr, labelStyle);
								GUI.Label(new Rect(10, 40, 420, 80), infoStr, labelStyle);
								if (GUI.Button(buttonBoundingBox, iter.Current.Value.image.texture, button50x50Style) && !fadingOut) {
									if (player.GetComponent<AccessoryManager>() != null) {
										if (itemSrc != null)
											itemSrc.Play();
										if (PlayerDataManager.itemEquipped == iter.Current.Key)
											player.GetComponent<AccessoryManager>().RemoveAccessory();
										else
											player.GetComponent<AccessoryManager>().SetAccessory(iter.Current.Key);
									}
								}
							} else {
								//blank button
								GUI.Button(buttonBoundingBox, "", button50x50Style);
							}
							if (!iter.MoveNext()) {
								iterdone = true;
								break;
							}
						}
						if (iterdone)
							break;
					}
				}

				//selectedItem = GUI.SelectionGrid(new Rect (Screen.width / 2 - menuWidth / 2 + 10, Screen.height / 2 - menuHeight / 2 + 15 + tabButtonHeight, scrollViewWidth, gridHeight), selectedItem, itemNames, inventoryGridX);				 
				GUI.EndGroup ();
			}

			//Map layout - scrollable list of locations across levels
			//You can teleport instantly to any checkpoint you have visited from here
			if (currentTab == MenuTab.Map) {

				GUI.skin = scrollbarSkin;
				scrollPosition2 = GUI.BeginScrollView (new Rect (Screen.width / 2 - menuWidth / 2 + 10, Screen.height / 2 - menuHeight / 2 + 15 + tabButtonHeight, scrollViewWidth, scrollViewHeight), 
				                                       scrollPosition2, new Rect (0, 0, scrollViewWidth - 20, scrollViewHeight*2.5f));
				GUI.skin = null;
				//Dictionary<int, List<int> > previousCheckpoints = player.GetComponent<PlayerDataManager>().previousCheckpoints;
				//Dictionary<int, List<int> > previousCheckpoints = PlayerDataManager.previousCheckpoints;
				//HashSet<int> previousCheckpoints = PlayerDataManager.previousCheckpoints;
				//Debug.Log (previousCheckpoints.Count);
				int i = 0;
				//foreach(KeyValuePair<int, List<int>> entry in previousCheckpoints)
				foreach (KeyValuePair<string, List<int> > entry in ImmutableData.GetLevelData())
				{
					bool flag = false;
					foreach (int checkID in entry.Value)
					{
						if (PlayerDataManager.previousCheckpoints.Contains(checkID)) {
							flag = true;
							break;
						}
					}
					if (!flag)
						continue;

					GUI.Label (new Rect (10, i*50+10, scrollViewWidth-10, 50), entry.Key, labelStyle);
					for(int j = 1; j < entry.Value.Count+1; j++)
					{
						if (!PlayerDataManager.previousCheckpoints.Contains (entry.Value[j-1]))
							continue;
						i++;
						//GUI.Label (new Rect (10, i*50+j*50+10, scrollViewWidth-10, 50),"Checkpoint "+entry.Value[j-1]);
						GUI.Label (new Rect (10, i*50+10, scrollViewWidth-10, 50),ImmutableData.GetCheckpointData()[entry.Value[j-1]].name, labelStyle);

						if(GUI.Button (new Rect(250,i*50+10,120,50),"Teleport", button120x50Style) && !fadingOut)
						{
							if (teleportSrc != null)
								teleportSrc.Play();
							teleportData = entry;
							teleportDataIndex = j-1;
							if (screenFadeObj != null) {
								screenFadeObj.GetComponent<ScreenFading>().fadeSpeed /= EPSILON;
								screenFadeObj.GetComponent<ScreenFading>().Transition(TeleportTransition, true);
							} else
								TeleportTransition();
							/*
							PlayerDataManager.loadedLevel = true;
							PlayerDataManager.lastCheckpoint = entry.Value[j-1];

							Application.LoadLevel(entry.Key);
							*/
							/*
							player.transform.position = Checkpoint.posCheckTable[entry.Value[j-1]];
							player.rigidbody2D.velocity = Vector2.zero;
							player.rigidbody2D.angularVelocity = 0f;
							if (Checkpoint.camCheckTable.ContainsKey(entry.Value[j-1]))
							{
								GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
								cam.GetComponent<CameraFollow>().LoadConfig(Checkpoint.camCheckTable[entry.Value[j-1]]);
							}
							*/
						}
					}
					//i+=entry.Value.Count;
					i++;
				}
				GUI.EndScrollView ();
			}

			menuWasOpen = true;
		} 
		else {
			menuWasOpen = false;
			Time.timeScale = 1;
		}
	}

	private int loadDataIndex=0;
	void LoadTransition() {
		PlayerDataManager.loadedLevel = false;
		BounceXmlSerializer.currentSaveFile = loadDataIndex;
		PlayerDataManager.LoadCurrentSave();
	}
	
	private KeyValuePair<string, List<int>> teleportData;
	private int teleportDataIndex=0;
	void TeleportTransition() {
		PlayerDataManager.loadedLevel = true;
		PlayerDataManager.lastCheckpoint = teleportData.Value[teleportDataIndex];
		Application.LoadLevel(teleportData.Key);
	}
}
