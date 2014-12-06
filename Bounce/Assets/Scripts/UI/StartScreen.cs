using UnityEngine;
using System.Collections;

public class StartScreen : MonoBehaviour {

	public Texture backgroundTexture;
	private GameObject screenFadeObj;

	void Start()
	{
		screenFadeObj = GameObject.FindGameObjectWithTag ("ScreenFader");
	}

	void OnGUI() {
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), backgroundTexture);

		if (GUI.Button (new Rect (Screen.width * 0.4f, Screen.height * 0.3f, Screen.width * 0.2f, Screen.height * 0.1f), "New Game")) {
			if (screenFadeObj != null)
				screenFadeObj.GetComponent<ScreenFading>().Transition(StartTransition);
			else
				StartTransition();
		}
		if (GUI.Button (new Rect (Screen.width * 0.4f, Screen.height * 0.45f, Screen.width * 0.2f, Screen.height * 0.1f), "Load Game")) {
			// load a saved game
		}
		if (GUI.Button (new Rect (Screen.width * 0.4f, Screen.height * 0.6f, Screen.width * 0.2f, Screen.height * 0.1f), "Credits")) {
			// go to credits scene
		}
	}

	void StartTransition()
	{
		Application.LoadLevel("MainHub");
	}

}
