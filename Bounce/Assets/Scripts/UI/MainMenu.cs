using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class MainMenu : MonoBehaviour
{

		// Use this for initialization
		void Start ()
		{
	
		}

		public bool showMenu = false;

		// Update is called once per frame
		void Update ()
		{
	
				if (Input.GetKeyDown (KeyCode.Escape))
						showMenu = !showMenu;
		}

		public float menuWidth = 500;
		public float menuHeight = 510;
		public float tabButtonWidth = 150;
		public float tabButtonHeight = 50;
		public float scrollViewWidth = 480;
		public float scrollViewHeight = 400;

		public enum Tab
		{
				Save,
				Inventory,
				Map}
		;

		public Tab currentTab = Tab.Save;
		Vector2 scrollPosition = Vector2.zero;
		Vector2 scrollPosition2 = Vector2.zero;

		private bool menuWasOpen = false;

		List<string> saveFileList = new List<string>();


	public int inventoryGridX = 5;
	public int selectedItem = 0;

	public string[] inventoryPlaceholder = {"Item1","Item2","Item3","Item4","Item5","Item6","Item7"};

		void OnGUI ()
		{
			if (showMenu) {

				Time.timeScale = 0;

				GUI.Box (new Rect (Screen.width / 2 - menuWidth / 2, Screen.height / 2 - menuHeight / 2, menuWidth, menuHeight), "");

				if (GUI.Button (new Rect (Screen.width / 2 - menuWidth / 2 + 10, Screen.height / 2 - menuHeight / 2 + 10, tabButtonWidth, tabButtonHeight), "Saves")) {
						currentTab = Tab.Save;
				}

				if (GUI.Button (new Rect (Screen.width / 2 - tabButtonWidth / 2, Screen.height / 2 - menuHeight / 2 + 10, tabButtonWidth, tabButtonHeight), "Inventory")) {
						currentTab = Tab.Inventory;
				}

				if (GUI.Button (new Rect (Screen.width / 2 + menuWidth / 2 - 10 - tabButtonWidth, Screen.height / 2 - menuHeight / 2 + 10, tabButtonWidth, tabButtonHeight), "Map")) {
						currentTab = Tab.Map;
				}


				if (currentTab == Tab.Save) {

				if(!menuWasOpen)
				{
					updateSaveFileList();
				}
				//scrollViewHeight = saveFileList.Count*50+10;
				scrollPosition = GUI.BeginScrollView (new Rect (Screen.width / 2 - menuWidth / 2 + 10, Screen.height / 2 - menuHeight / 2 + 15 + tabButtonHeight, scrollViewWidth, scrollViewHeight), scrollPosition, new Rect (0, 0, scrollViewWidth - 20, saveFileList.Count*50+10));

				if(GUI.Button (new Rect(10,10,100,50),"New File"))
				{
					XmlSerialzer.currentSaveFile = saveFileList.Count-1;
					GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDataManager>().saveCurrent();
					updateSaveFileList();
				}

				for(int i = 1; i < saveFileList.Count+1; i++)
				{
					string s = saveFileList[i-1];
					GUI.Label (new Rect (10, i*70+10, scrollViewWidth-10, 50),"File "+s.ElementAt(s.Length-1));
					if(GUI.Button (new Rect(50,i*60+10,100,50),"Save"))
					{
						XmlSerialzer.currentSaveFile = int.Parse(s.ElementAt(s.Length-1).ToString());
						GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDataManager>().saveCurrent();
						updateSaveFileList();
					}
					if(GUI.Button (new Rect(160,i*60+10,100,50),"Load"))
					{
						XmlSerialzer.currentSaveFile = int.Parse(s.ElementAt(s.Length-1).ToString());
						PlayerDataManager.loadedLevel = false;
						GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDataManager>().LoadCurrentSave();
						updateSaveFileList();
					}

				}
							
					GUI.EndScrollView ();
				}

				if (currentTab == Tab.Inventory) {
				Inventory inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDataManager>().inventory;
				int count = inventory.items.Length;
				float gridHeight = (float)count/inventoryGridX;
				gridHeight = Mathf.Ceil(gridHeight*150);
				string[] itemNames = {"1","2","3"};
				selectedItem = GUI.SelectionGrid(new Rect (Screen.width / 2 - menuWidth / 2 + 10, Screen.height / 2 - menuHeight / 2 + 15 + tabButtonHeight, scrollViewWidth, gridHeight), selectedItem,itemNames, inventoryGridX);

				}

				if (currentTab == Tab.Map) {

				scrollPosition2 = GUI.BeginScrollView (new Rect (Screen.width / 2 - menuWidth / 2 + 10, Screen.height / 2 - menuHeight / 2 + 15 + tabButtonHeight, scrollViewWidth, scrollViewHeight), scrollPosition2, new Rect (0, 0, scrollViewWidth - 20, scrollViewHeight * 4));
				Dictionary<int,List<int>> previousCheckpoints = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDataManager>().previousCheckpoints;
				Debug.Log (previousCheckpoints.Count);
				int i = 0;
				foreach(KeyValuePair<int, List<int>> entry in previousCheckpoints)
				{
					GUI.Label (new Rect (10, i*50+10, scrollViewWidth-10, 50),"Level "+entry.Key);
					for(int j = 1; j < entry.Value.Count+1; j++)
					{
						GUI.Label (new Rect (10, i*50+j*50+10, scrollViewWidth-10, 50),"Checkpoint "+entry.Value[j-1]);
						if(GUI.Button (new Rect(150,i*50+j*50+10,100,50),"Teleport"))
						{

						}
					}
					i+=entry.Value.Count;
					i++;
				}

				GUI.EndScrollView ();
			}
			menuWasOpen = true;
		} else {
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
				saveFileList.Add (s);
			}
		}
	}
}
