using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class StartScreen : MonoBehaviour {

	public AudioSource navigateSrc;
	public AudioSource loadSrc;

	//public Texture backgroundTexture;
	private GameObject screenFadeObj;
	private List<string> saveFileList = new List<string>();

	private bool loadPanelVisible = false;

	Vector2 scrollPosition = Vector2.zero;

	void Start()
	{
		UpdateSaveFileList();
		screenFadeObj = GameObject.FindGameObjectWithTag ("ScreenFader");
		//navigateSrc = transform.GetChild (0).audio;
		//loadSrc = transform.GetChild (1).audio;
	}

	void OnGUI() {
		//GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), backgroundTexture);
		if (loadPanelVisible) {
			DrawLoadPanel();
		} else {
			DrawMainPanel();
		}
	}

	void StartTransition()
	{
		Application.LoadLevel("Pier");
	}

	void ShowLoadPanelTransition() {
		loadPanelVisible = !loadPanelVisible;
	}

	void DrawMainPanel() {
		if (GUI.Button (new Rect (Screen.width * 0.1f, Screen.height * 0.8f, Screen.width * 0.2f, Screen.height * 0.1f), "New Game")) {
			if (loadSrc != null)
				loadSrc.Play();
			if (screenFadeObj != null)
				screenFadeObj.GetComponent<ScreenFading>().Transition(StartTransition, true);
			else
				StartTransition();
		}
		if (GUI.Button (new Rect (Screen.width * 0.4f, Screen.height * 0.8f, Screen.width * 0.2f, Screen.height * 0.1f), "Load Game")) {
			if (navigateSrc != null)
				navigateSrc.Play();
			if (screenFadeObj != null)
				screenFadeObj.GetComponent<ScreenFading>().Transition(ShowLoadPanelTransition, true);
			else
				ShowLoadPanelTransition();
		}
		if (GUI.Button (new Rect (Screen.width * 0.7f, Screen.height * 0.8f, Screen.width * 0.2f, Screen.height * 0.1f), "Credits")) {
			if (navigateSrc != null)
				navigateSrc.Play();
			// go to credits scene
		}
	}

	void DrawLoadPanel() {
		GUI.Box (new Rect (0.1f * Screen.width, 0.1f * Screen.height, 0.8f * Screen.width, 0.8f * Screen.height), "");

		scrollPosition = GUI.BeginScrollView (new Rect (0.1f * Screen.width, 0.1f * Screen.height, 0.8f * Screen.width, 0.8f * Screen.height),
		                                      scrollPosition, new Rect (0, 0, Screen.width * 0.75f, saveFileList.Count*50f+10f));

		// A list of all our save files
		for(int i = 1; i < saveFileList.Count+1; i++)
		{
			//string s = saveFileList[i-1];
			int saveFileIndex = i-1;
			GUI.Label (new Rect (10, i*70+10, Screen.width * 0.75f-10, 50),"File "+saveFileIndex);
			if(GUI.Button (new Rect(170,i*60+10,120,50),"Load"))
			{
				if (loadSrc != null)
					loadSrc.Play();
				loadDataIndex = saveFileIndex;
				if (screenFadeObj != null) {
					//screenFadeObj.GetComponent<ScreenFading>().fadeSpeed /= EPSILON;
					screenFadeObj.GetComponent<ScreenFading>().Transition(LoadTransition, true);
				} else
					LoadTransition();
				
			}
		}						
		GUI.EndScrollView ();
	}

	void UpdateSaveFileList()
	{
		if (!Directory.Exists(XmlSerialzer.saveDirectory)) {
			saveFileList.Clear();
			return;
		}
		
		//string root = Path.GetDirectoryName (Application.dataPath);
		List<string> FullFileList = Directory.GetFiles (XmlSerialzer.saveDirectory, 
		                                                XmlSerialzer.savePrefix + "*" + XmlSerialzer.saveSuffix, 
		                                                SearchOption.TopDirectoryOnly).ToList();
		saveFileList.Clear ();
		saveFileList.AddRange (FullFileList);
		saveFileList.Sort ();
	}

	private int loadDataIndex=0;
	void LoadTransition() {
		PlayerDataManager.loadedLevel = false;
		XmlSerialzer.currentSaveFile = loadDataIndex;
		PlayerDataManager.LoadCurrentSave();
	}
}
