using UnityEngine;
using System.Collections;

public class PowerUpTime : MonoBehaviour {
	public GameObject manager;
	public string timetext = "Time: ";
	public float timer = 0f;
	public float time = 1f;
	public GUIStyle style;
	public Texture2D bgImage;
	public Texture2D fgImage;
	public float timerBarLength;

	void Start()
	{
				//player = GameObject.FindGameObjectWithTag ("Player");
		//instance = this;
		timerBarLength = Screen.width / 2;
		manager = gameObject;
		PowerupManager pum = manager.GetComponent<PowerupManager> ();
		timer = pum.powerupTimer;
		timer = pum.powerupTime;
	}

	void Update () {
		manager = gameObject;
		PowerupManager pum = manager.GetComponent<PowerupManager> ();
		timer = pum.powerupTimer;
		timer = pum.powerupTime;
	}

	void OnGUI(){
		GUI.Label (new Rect (735, 12, 60, 20), timetext + timer.ToString(), style);
		//GUI.Box (new Rect (10, 10, 0.001 * Screen.width * timer, 0.1 * Screen.height), "TIMER", style);
		//instance.timerBarRect.width = TimerBarWidth * (((int)timer) / 200);
		//instance.timerBarRect.height = 20;

		//instance.timerBarBackgroundRect.width = TimerBarWidth;
		//instance.timerBarBackground.height = 20;

		//GUI.DrawTexture (timerBarRect, timerBar);
		//GUI.DrawTexture (timerBarBackgroundRect, timerBarBackground);

		//GUI.Label (timerBarLabelRect, "TIME");

		//GUI.Box (new Rect (10, 10, 300, 20), new GUIContent(bgImage));
		
	}

}






