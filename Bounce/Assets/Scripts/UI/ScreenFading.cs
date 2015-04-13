using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(GUITexture))]
public class ScreenFading : MonoBehaviour {

	public Color opaqueColor = Color.black;
	public float fadeSpeed = 4.0f;
	public bool fadeOutOnStart = false;

	public bool fadeMusic = false;
	public GameObject musicObj = null;

	public bool useOnGUI = false;
	public GUIStyle style;

	private bool fadingIn = false;
	private bool fadingOut = false;
	private float inThreshold = 0.95f;
	private float outThreshold = 0.05f;
	private Action transitionFunc = null;
	private Color mColor;

	void Awake()
	{
		guiTexture.pixelInset = new Rect (0f, 0f, Screen.width, Screen.height);
		guiTexture.color = Color.clear;
		if (fadeOutOnStart) {
			fadeMusic = true;
			if (musicObj != null)
				musicObj.audio.volume = 0.0f;
			mColor = opaqueColor;
			if (!useOnGUI)
				guiTexture.color = opaqueColor;
			fadingOut = true;
		}
		else {
			mColor = Color.clear;
			if (!useOnGUI)
				guiTexture.color = Color.clear;
		}
	}

	// Update is called once per frame
	void Update () 
	{
		if (fadingIn)
		{
			if (fadeMusic && musicObj != null)
			{
				musicObj.audio.volume = Mathf.Lerp(musicObj.audio.volume, 0.0f, fadeSpeed * Time.deltaTime);
			}
			mColor = Color.Lerp(mColor, opaqueColor, fadeSpeed * Time.deltaTime);
			if (mColor.a >= inThreshold)
			{
				mColor = opaqueColor;
				fadingIn = false;
				if (transitionFunc != null)
				{
					transitionFunc();
					fadingOut = true;
				}
			}
			if (!useOnGUI) {
				guiTexture.color = mColor;
			}
		}
		else if (fadingOut)
		{
			if (fadeMusic && musicObj != null)
			{
				musicObj.audio.volume = Mathf.Lerp(musicObj.audio.volume, 1.0f, fadeSpeed * Time.deltaTime);
			}
			mColor = Color.Lerp(mColor, Color.clear, fadeSpeed * Time.deltaTime);
			if (mColor.a <= outThreshold)
			{
				mColor = Color.clear;
				fadingOut = false;
			}
			if (!useOnGUI) {
				guiTexture.color = mColor;
			}
		}
	}

	//very ugly work-around to render over menu elements
	public void OnGUI() {
		if (useOnGUI) {
			GUI.depth = -1;
			GUI.color = mColor;
			GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "", style);
			GUI.color = Color.white;
		}
	}

	public void Transition(Action transitionAction, bool music = false)
	{
		if (fadingIn)
			return;		//deny a new fade in
		fadingIn = true;
		fadeMusic = music;
		transitionFunc = transitionAction;
	}

	public bool IsTransitioning()
	{
		return fadingIn;
	}
}
