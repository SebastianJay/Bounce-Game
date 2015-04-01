using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class StartScreen : MonoBehaviour {

	public AudioSource navigateSrc;
	public AudioSource loadSrc;
	public GUIStyle style;

	//public Texture backgroundTexture;
	private GameObject screenFadeObj;
	private List<string> saveFileList = new List<string>();

	private bool loadPanelVisible = false;
	private bool savePanelVisible = false;
	Vector2 scrollPosition = Vector2.zero;

	void Start()
	{
		UpdateSaveFileList();
		screenFadeObj = GameObject.FindGameObjectWithTag ("ScreenFader");
	}

	void OnGUI() {
		//GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), backgroundTexture);
		GUI.depth = 0;
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
		bool fading = screenFadeObj != null && screenFadeObj.GetComponent<ScreenFading> ().IsTransitioning();
		if (GUI.Button (new Rect (Screen.width * 0.1f, Screen.height * 0.8f, Screen.width * 0.2f, Screen.height * 0.1f), "New Game") && !fading) {
			if (loadSrc != null)
				loadSrc.Play();
			if (screenFadeObj != null)
				screenFadeObj.GetComponent<ScreenFading>().Transition(StartTransition, true);
			else
				StartTransition();
		}
		if (GUI.Button (new Rect (Screen.width * 0.4f, Screen.height * 0.8f, Screen.width * 0.2f, Screen.height * 0.1f), "Load Game") && !fading) {
			if (navigateSrc != null)
				navigateSrc.Play();
			if (screenFadeObj != null)
				screenFadeObj.GetComponent<ScreenFading>().Transition(ShowLoadPanelTransition, false);
			else
				ShowLoadPanelTransition();
		}
		if (GUI.Button (new Rect (Screen.width * 0.7f, Screen.height * 0.8f, Screen.width * 0.2f, Screen.height * 0.1f), "Credits") && !fading) {
			if (navigateSrc != null)
				navigateSrc.Play();
			// go to credits scene
		}
	}

	void DrawLoadPanel() {
		bool fading = screenFadeObj != null && screenFadeObj.GetComponent<ScreenFading> ().IsTransitioning();
		GUI.Box (new Rect (0.1f * Screen.width, 0.1f * Screen.height, 0.8f * Screen.width, 0.7f * Screen.height), "");

		scrollPosition = GUI.BeginScrollView (new Rect (0.1f * Screen.width, 0.1f * Screen.height, 0.8f * Screen.width, 0.8f * Screen.height),
		                                      scrollPosition, new Rect (0, 0, Screen.width * 0.75f, saveFileList.Count*50f+10f));

		// A list of all our save files
		for(int i = 0; i < saveFileList.Count; i++)
		{
			int saveFileIndex = i;
			GUI.Label (new Rect (10, i*70+10, Screen.width * 0.75f-10, 50),"File "+saveFileIndex);
			if(GUI.Button (new Rect(170,i*60+10,120,50),"Load") && !fading)
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
		if (GUI.Button (new Rect (0.1f * Screen.width, 0.8f * Screen.height, 0.8f * Screen.width, 0.1f * Screen.height), "Back") && !fading) {
			if (navigateSrc != null)
				navigateSrc.Play();
			if (screenFadeObj != null)
				screenFadeObj.GetComponent<ScreenFading>().Transition(ShowLoadPanelTransition, false);
			else
				ShowLoadPanelTransition();
		}
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
