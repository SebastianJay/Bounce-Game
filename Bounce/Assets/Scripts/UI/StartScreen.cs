using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class StartScreen : MonoBehaviour {

	public AudioSource navigateSrc;
	public AudioSource loadSrc;

	public Texture2D cursorTexture;
	public CursorMode cursorMode = CursorMode.Auto;
	public Vector2 hotSpot = Vector2.zero;

	public GUIStyle panelStyle;
	public GUIStyle labelStyle;
	public GUIStyle button120x50Style;
	public GUIStyle button320x90Style;
	public GUIStyle button1280x90Style;

	public GUISkin scrollbarSkin;

	//public Texture backgroundTexture;
	private GameObject screenFadeObj;
	private List<string> saveFileList = new List<string>();

	private bool loadPanelVisible = false;
	private bool creditsPanelVisible = false;
	Vector2 scrollPosition = Vector2.zero;

	void Start()
	{
		UpdateSaveFileList();
		screenFadeObj = GameObject.FindGameObjectWithTag ("ScreenFader");
	}

	void OnMouseEnter() {
		Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
	}
	
	void OnMouseExit() {
		Cursor.SetCursor(null, Vector2.zero, cursorMode);
	}

	void OnGUI() {
		//GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), backgroundTexture);
		GUI.depth = 0;
		if (loadPanelVisible) {
			DrawLoadPanel();
		} else if (creditsPanelVisible) {
			DrawCreditsPanel();
		} else {
			DrawMainPanel();
		}
	}

	void DrawMainPanel() {
		bool fading = screenFadeObj != null && screenFadeObj.GetComponent<ScreenFading> ().IsTransitioning();
		if (GUI.Button (new Rect (Screen.width * 0.1f, Screen.height * 0.8f, Screen.width * 0.2f, Screen.height * 0.1f), "New Game", button320x90Style) && !fading) {
			if (loadSrc != null)
				loadSrc.Play();
			if (screenFadeObj != null)
				screenFadeObj.GetComponent<ScreenFading>().Transition(StartTransition, true);
			else
				StartTransition();
		}
		if (GUI.Button (new Rect (Screen.width * 0.4f, Screen.height * 0.8f, Screen.width * 0.2f, Screen.height * 0.1f), "Load Game", button320x90Style) && !fading) {
			if (navigateSrc != null)
				navigateSrc.Play();
			if (screenFadeObj != null)
				screenFadeObj.GetComponent<ScreenFading>().Transition(ShowLoadPanelTransition, false);
			else
				ShowLoadPanelTransition();
		}
		if (GUI.Button (new Rect (Screen.width * 0.7f, Screen.height * 0.8f, Screen.width * 0.2f, Screen.height * 0.1f), "Credits", button320x90Style) && !fading) {
			if (navigateSrc != null)
				navigateSrc.Play();
			if (screenFadeObj != null)
				screenFadeObj.GetComponent<ScreenFading>().Transition(ShowCreditsPanelTransition, false);
			else
				ShowCreditsPanelTransition();
			// go to credits scene
		}
	}

	void DrawLoadPanel() {
		bool fading = screenFadeObj != null && screenFadeObj.GetComponent<ScreenFading> ().IsTransitioning();
		GUI.Box (new Rect (0.1f * Screen.width, 0.1f * Screen.height, 0.8f * Screen.width, 0.7f * Screen.height), "", panelStyle);

		GUI.skin = scrollbarSkin;
		scrollPosition = GUI.BeginScrollView (new Rect (0.1f * Screen.width, 0.1f * Screen.height, 0.8f * Screen.width, 0.8f * Screen.height),
		                                      scrollPosition, new Rect (0, 0, Screen.width * 0.75f, saveFileList.Count*50f+10f));
		GUI.skin = null;

		// A list of all our save files
		for(int i = 0; i < saveFileList.Count; i++)
		{
			int saveFileIndex = i;
			GUI.Label (new Rect (10, i*60+10, Screen.width * 0.75f-10, 50),"File "+saveFileIndex,labelStyle);
			if(GUI.Button (new Rect(170,i*60+10,120,50),"Load",button120x50Style) && !fading)
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
		if (GUI.Button (new Rect (0.1f * Screen.width, 0.8f * Screen.height, 0.8f * Screen.width, 0.1f * Screen.height), "Back", button1280x90Style) && !fading) {
			if (navigateSrc != null)
				navigateSrc.Play();
			if (screenFadeObj != null)
				screenFadeObj.GetComponent<ScreenFading>().Transition(ShowLoadPanelTransition, false);
			else
				ShowLoadPanelTransition();
		}
	}

	void DrawCreditsPanel() {
		bool fading = screenFadeObj != null && screenFadeObj.GetComponent<ScreenFading> ().IsTransitioning();
		GUI.Box (new Rect (0.1f * Screen.width, 0.1f * Screen.height, 0.8f * Screen.width, 0.7f * Screen.height), "", panelStyle);


		GUI.skin = scrollbarSkin;
		scrollPosition = GUI.BeginScrollView (new Rect (0.1f * Screen.width, 0.1f * Screen.height, 0.8f * Screen.width, 0.8f * Screen.height),
		                                      scrollPosition, new Rect (0, 0, Screen.width * 0.75f, saveFileList.Count*50f+10f));
		GUI.skin = null;

		//we'll hardcode the literals
		string header = "Bounce was created by student game developers at the University of Virginia.\n" +
			"All assets are original, with the exception of sound effects and fonts, which were freely provided by nice people on the Internet. Thank you people!\n" +
			"A list of people who contributed to this project in some form or fashion is provided below, in no particular order:";
		string[] contributors = {"Jay Sebastian", "Julian McClinton", "Raymond Zhao", "Carter Hall",
								 "Ronak Dhaddha", "Rob Shimshock", "Reid Bixler", "Joseph Baik",
								 "Tyler Anderson", "Casey Horton", "Stephen Dwyer", "Brandon Bright",
								 "Anny Wang", "Scott Kirkpatrick", "Emily Costigan", "Brian Team",
								 "Nathan Chatham", "Lane Spangler", "Jess Platter", "David Wert",
								 "Trad Groover", "Madelyn Luansing", "Isaac Tessler"};
		string footer = "You can check out the project at https://github.com/SebastianJay/Bounce-Game";
		GUI.Label (new Rect (20, 10, Screen.width * 0.75f - 20, 30), header, labelStyle);
		for (int i = 0; i < contributors.Length; i++) {
			GUI.Label (new Rect (20+(i/8)*180, 110+(i%8)*20, 200, 15), contributors[i], labelStyle);
		}
		GUI.Label (new Rect(20, 270, Screen.width * 0.75f-20, 15), footer, labelStyle);
		GUI.EndScrollView ();
		if (GUI.Button (new Rect (0.1f * Screen.width, 0.8f * Screen.height, 0.8f * Screen.width, 0.1f * Screen.height), "Back", button1280x90Style) && !fading) {
			if (navigateSrc != null)
				navigateSrc.Play();
			if (screenFadeObj != null)
				screenFadeObj.GetComponent<ScreenFading>().Transition(ShowCreditsPanelTransition, false);
			else
				ShowCreditsPanelTransition();
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

	void StartTransition()
	{
		ImmutableData.Init();
		Application.LoadLevel("Pier");
	}
	
	void ShowLoadPanelTransition() {
		loadPanelVisible = !loadPanelVisible;
	}

	void ShowCreditsPanelTransition() {
		creditsPanelVisible = !creditsPanelVisible;
	}

	private int loadDataIndex=0;
	void LoadTransition() {
		ImmutableData.Init();
		PlayerDataManager.loadedLevel = false;
		XmlSerialzer.currentSaveFile = loadDataIndex;
		PlayerDataManager.LoadCurrentSave();
	}
}
