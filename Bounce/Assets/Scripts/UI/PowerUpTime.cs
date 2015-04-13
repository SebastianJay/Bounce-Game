using UnityEngine;
using System.Collections;

public class PowerUpTime : MonoBehaviour {
	//public GameObject player;
	//public GameObject manager;
	//public string timetext = "Time: ";

	//these values are updated from the powerup manager
	public float timer = 0f;
	public float time = 1f;
	public float barHorizontalLength = 0.5f;
	public float barVerticalLength = 0.1f;
	public float frameOffsetFactor = 0.0f;

	//public GUIStyle style;
	//public Texture2D bgImage;
	//public Texture2D fgImage;
	//public float timerBarLength;
	
	//float barDisplay = 0; 
	//Vector2 pos = new Vector2(20,40);
	//Vector2 size = new Vector2(60,20);
//	public Texture2D progressBarEmpty;
//	public Texture2D progressBarFull;
	public GUITexture pbbackground;
	public GUITexture powerbar;
	
	void Start()
	{
		//player = GameObject.FindGameObjectWithTag ("Player");
		//instance = this;
		//timerBarLength = Screen.width / 2;
		//manager = gameObject;
		//PowerupManager pum = player.GetComponent<PowerupManager> ();
		//timer = pum.powerupTimer;
		//time = pum.powerupTime;
	}
	
	void Update () {
		//manager = gameObject;
		//PowerupManager pum = player.GetComponent<PowerupManager> ();
		//timer = pum.powerupTimer;
		//time = pum.powerupTime;

		if((time - timer)/time > 0.001f && timer > 0 ){
			pbbackground.enabled = true;
			powerbar.enabled = true;

			float frameOffset = frameOffsetFactor * barHorizontalLength;
			pbbackground.transform.position = new Vector3(barHorizontalLength / 2, 1f - barVerticalLength/2, -2f);
			pbbackground.transform.localScale = new Vector3(barHorizontalLength, barVerticalLength, 1f);
			powerbar.transform.position = new Vector3((frameOffset + ((time-timer)/time) * (barHorizontalLength/2 - frameOffset)),
			                                          1f - barVerticalLength/2, -1f);
			powerbar.transform.localScale = new Vector3(((time-timer)/time) * (barHorizontalLength - 2*frameOffset),
			                                            barVerticalLength, 1f);
		}
		else{
			pbbackground.enabled = false;
			powerbar.enabled = false;
			//pbbackground.transform.position = new Vector3(0, 2, -2);
			//powerbar.transform.position = new Vector3(0, 2, -1);
		}
	}
	
	void OnGUI(){
//		GUI.Label (new Rect (735, 12, 60, 20), timetext + timer.ToString(), style);
//		GUI.Box (new Rect (10, 10, 0.001 * Screen.width * timer, 0.1 * Screen.height), "TIMER", style);
//		instance.timerBarRect.width = TimerBarWidth * (((int)timer) / 200);
//		instance.timerBarRect.height = 20;
		
//		instance.timerBarBackgroundRect.width = TimerBarWidth;
		//instance.timerBarBackground.height = 20;
		
//		GUI.DrawTexture (timerBarRect, timerBar);
//		GUI.DrawTexture (timerBarBackgroundRect, timerBarBackground);
		
//		GUI.Label (timerBarLabelRect, "TIME");
		
//		GUI.Box (new Rect (10, 10, 300, 20), new GUIContent(bgImage));

//		if((time - timer)/time > 0.001f && timer > 0){	
//			GUI.BeginGroup (new Rect (0, 0, Screen.width, Screen.height));
//				GUI.Box (new Rect (10,10, timerBarLength, 20), "BACKGROUND");
//		
//				GUI.BeginGroup (new Rect (0, 0, Screen.width, Screen.height));
//					GUI.Box ( new Rect (10,10, ((time-timer)/time) *timerBarLength, 20), "POWER");
//				GUI.EndGroup ();
//			GUI.EndGroup ();
//		}

	}
	
}







