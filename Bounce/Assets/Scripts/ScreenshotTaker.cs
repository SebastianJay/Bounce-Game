using UnityEngine;
using System.Collections;
using System.IO;

public class ScreenshotTaker : MonoBehaviour {

	public string path = "../Concept_Dropbox/Screenshots/";
	public string prefix = "Bounce_screen_";
	public string suffix = ".png";
	public int supersize = 0;

	private int index;

	void Start() {
		string[] files = Directory.GetFiles(path, prefix + "*" + suffix);
		index = files.Length;
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.P))
		{
			Application.CaptureScreenshot(path + prefix + index.ToString() + suffix, supersize);
			index++;
		}
	}
}
