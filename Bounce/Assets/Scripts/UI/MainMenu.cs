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

	public MenuTab currentTab = MenuTab.Save;
	public bool showMenu = false;
	
	public float menuWidth = 500;
	public float menuHeight = 510;
	public float tabButtonWidth = 150;
	public float tabButtonHeight = 50;
	public float scrollViewWidth = 480;
	public float scrollViewHeight = 400;

	Vector2 scrollPosition = Vector2.zero;
	Vector2 scrollPosition2 = Vector2.zero;
	
	private bool menuWasOpen = false;		//represents the first "tick" of active menu
	private GameObject player;
	List<string> saveFileList = new List<string>();

	//temp vars
	public int inventoryGridX = 5;
	public int selectedItem = 0;
	public string[] inventoryPlaceholder = {"Item1","Item2","Item3","Item4","Item5","Item6","Item7"};

	// Use this for initialization
	void Start ()
	{
		player = GameObject.FindGameObjectWithTag("Player");
	}

	// Update is called once per frame
	void Update ()
	{
		if (Input.GetButtonDown("Menu"))
			showMenu = !showMenu;
	}

	void OnGUI ()
	{
		if (showMenu) {

			//Freeze the game if the menu is active
			Time.timeScale = 0;

			//Main menu frame
			GUI.Box (new Rect (Screen.width / 2 - menuWidth / 2, Screen.height / 2 - menuHeight / 2, menuWidth, menuHeight), "");

			//Buttons for the three tabs
			if (GUI.Button (new Rect (Screen.width / 2 - menuWidth / 2 + 10, Screen.height / 2 - menuHeight / 2 + 10, tabButtonWidth, tabButtonHeight), "Saves")) {
					currentTab = MenuTab.Save;
			}
			if (GUI.Button (new Rect (Screen.width / 2 - tabButtonWidth / 2, Screen.height / 2 - menuHeight / 2 + 10, tabButtonWidth, tabButtonHeight), "Inventory")) {
					currentTab = MenuTab.Inventory;
			}
			if (GUI.Button (new Rect (Screen.width / 2 + menuWidth / 2 - 10 - tabButtonWidth, Screen.height / 2 - menuHeight / 2 + 10, tabButtonWidth, tabButtonHeight), "Map")) {
					currentTab = MenuTab.Map;
			}

			//The following layout of the menu is dependent on which tab is open
			//Save tab layout - scrollable list of save files
			//from which you can save, load, or create a new save
			if (currentTab == MenuTab.Save) {

				if(!menuWasOpen)
				{
					updateSaveFileList();
				}

				scrollPosition = GUI.BeginScrollView (new Rect (Screen.width / 2 - menuWidth / 2 + 10, Screen.height / 2 - menuHeight / 2 + 15 + tabButtonHeight, scrollViewWidth, scrollViewHeight), scrollPosition, new Rect (0, 0, scrollViewWidth - 20, saveFileList.Count*50+10));

				// A button for creating a new save file
				if(GUI.Button (new Rect(10,10,100,50),"New File"))
				{
					XmlSerialzer.currentSaveFile = saveFileList.Count-1;
					player.GetComponent<PlayerDataManager>().saveCurrent();
					updateSaveFileList();
				}

				// A list of all our save files
				for(int i = 1; i < saveFileList.Count+1; i++)
				{
					string s = saveFileList[i-1];
					GUI.Label (new Rect (10, i*70+10, scrollViewWidth-10, 50),"File "+s.ElementAt(s.Length-1));
					if(GUI.Button (new Rect(50,i*60+10,100,50),"Save"))
					{
						XmlSerialzer.currentSaveFile = int.Parse(s.ElementAt(s.Length-1).ToString());
						player.GetComponent<PlayerDataManager>().saveCurrent();
						updateSaveFileList();
					}
					if(GUI.Button (new Rect(160,i*60+10,100,50),"Load"))
					{
						XmlSerialzer.currentSaveFile = int.Parse(s.ElementAt(s.Length-1).ToString());
						PlayerDataManager.loadedLevel = false;
						player.GetComponent<PlayerDataManager>().LoadCurrentSave();
						updateSaveFileList();
					}
				}						
				GUI.EndScrollView ();
			}

			//Inventory tab layout - grid of items
			//hovering over an item yields its name and description
			if (currentTab == MenuTab.Inventory) {
				Inventory inventory = player.GetComponent<PlayerDataManager>().inventory;
				int count = inventory.items.Length;
				float gridHeight = (float)count/inventoryGridX;
				gridHeight = Mathf.Ceil(gridHeight*150);
				string[] itemNames = {"1","2","3"};
				selectedItem = GUI.SelectionGrid(new Rect (Screen.width / 2 - menuWidth / 2 + 10, Screen.height / 2 - menuHeight / 2 + 15 + tabButtonHeight, scrollViewWidth, gridHeight), selectedItem, itemNames, inventoryGridX);
			}

			//Map layout - scrollable list of locations across levels
			//You can teleport instantly to any checkpoint you have visited from here
			if (currentTab == MenuTab.Map) {

				scrollPosition2 = GUI.BeginScrollView (new Rect (Screen.width / 2 - menuWidth / 2 + 10, Screen.height / 2 - menuHeight / 2 + 15 + tabButtonHeight, scrollViewWidth, scrollViewHeight), scrollPosition2, new Rect (0, 0, scrollViewWidth - 20, scrollViewHeight * 4));
				Dictionary<int,List<int>> previousCheckpoints = player.GetComponent<PlayerDataManager>().previousCheckpoints;
				//Debug.Log (previousCheckpoints.Count);
				int i = 0;
				foreach(KeyValuePair<int, List<int>> entry in previousCheckpoints)
				{
					GUI.Label (new Rect (10, i*50+10, scrollViewWidth-10, 50),"Level "+entry.Key);
					for(int j = 1; j < entry.Value.Count+1; j++)
					{
						GUI.Label (new Rect (10, i*50+j*50+10, scrollViewWidth-10, 50),"Checkpoint "+entry.Value[j-1]);
						if(GUI.Button (new Rect(150,i*50+j*50+10,100,50),"Teleport"))
						{
							player.transform.position = Checkpoint.posCheckTable[entry.Value[j-1]];
							player.rigidbody2D.velocity = Vector2.zero;
							player.rigidbody2D.angularVelocity = 0f;
							if (Checkpoint.camCheckTable.ContainsKey(entry.Value[j-1]))
							{
								GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
								cam.GetComponent<CameraFollow>().LoadConfig(Checkpoint.camCheckTable[entry.Value[j-1]]);
							}
						}
					}
					i+=entry.Value.Count;
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

	void updateSaveFileList()
	{
		string root = Path.GetDirectoryName (Application.dataPath);
		List<string> FullFileList = Directory.GetFiles (root, "*.*", SearchOption.AllDirectories).Select (file => file.Replace (root, "")).ToList ();
		saveFileList.Clear ();
		foreach (string s in FullFileList) 
		{
			if(s.Contains("BounceSaveData"))
			{
				saveFileList.Add(s);
			}
		}
	}
}
