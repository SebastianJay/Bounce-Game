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

	private bool fadingIn = false;
	private bool fadingOut = false;
	private float inThreshold = 0.95f;
	private float outThreshold = 0.05f;
	private Action transitionFunc = null;
	
	void Awake()
	{
		guiTexture.pixelInset = new Rect (0f, 0f, Screen.width, Screen.height);
		guiTexture.color = Color.clear;
		if (fadeOutOnStart)
		{
			fadeMusic = true;
			if (musicObj != null)
				musicObj.audio.volume = 0.0f;
			guiTexture.color = opaqueColor;
			fadingOut = true;
		}
	}

	// Update is called once per frame
	void Update () 
	{
		if (fadingIn)
		{
			guiTexture.color = Color.Lerp(guiTexture.color, opaqueColor, fadeSpeed * Time.deltaTime);
			if (fadeMusic && musicObj != null)
			{
				musicObj.audio.volume = Mathf.Lerp(musicObj.audio.volume, 0.0f, fadeSpeed * Time.deltaTime);
			}
			if (guiTexture.color.a >= inThreshold)
			{
				guiTexture.color = opaqueColor;
				fadingIn = false;
				if (transitionFunc != null)
				{
					transitionFunc();
					fadingOut = true;
				}
			}
		}
		else if (fadingOut)
		{
			guiTexture.color = Color.Lerp(guiTexture.color, Color.clear, fadeSpeed * Time.deltaTime);
			if (fadeMusic && musicObj != null)
			{
				musicObj.audio.volume = Mathf.Lerp(musicObj.audio.volume, 1.0f, fadeSpeed * Time.deltaTime);
			}
			if (guiTexture.color.a <= outThreshold)
			{
				guiTexture.color = Color.clear;
				fadingOut = false;
			}
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
