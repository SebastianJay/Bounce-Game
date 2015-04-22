using UnityEngine;
using System.Collections;

public class PowerUpTime : MonoBehaviour {
	//lots of magic numbers here calculated from aspect ratios
	public Vector2 barPosStart = new Vector2(0.2f, 1f - (0.4f * 0.0877712f / 2));
	public Vector2 barDimensions = new Vector2(0.4f, 0.4f * 0.0877712f); 
	//foreground nums are relative to background
	//public Vector2 foregroundPosStart = new Vector2(0.056213f, 0.187266f);
	public Vector2 foregroundDimensions = new Vector2(0.891519f, 0.606742f);	

	public GUITexture pbbackground;
	public GUITexture pbforeground;

	//these values are updated from the powerup manager
	private float timer = 0f;
	private float time = 1f;

	private Vector2 foregroundDimsReal;
	private Vector2 foregroundPosReal;
	private Texture2D generatedTexture;

	void Start()
	{
		float resizedAspect = barDimensions.x / barDimensions.y;
		pbbackground.transform.position = new Vector3 (barPosStart.x, barPosStart.y, -2f);
		pbbackground.transform.localScale = new Vector3(barDimensions.x, barDimensions.y, 1f);
		foregroundDimsReal = new Vector2(barDimensions.x * foregroundDimensions.x, barDimensions.y * foregroundDimensions.y);
		//foregroundPosReal = new Vector2(barPosStart.x + foregroundPosStart.x * barDimensions.x, barPosStart.y - foregroundPosStart.y * barDimensions.y);
		foregroundPosReal = barPosStart;
		pbbackground.enabled = false;
		pbforeground.enabled = false;
		generatedTexture = new Texture2D (1, 1);
		pbforeground.texture = generatedTexture;
		pbforeground.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
	}
	
	void Update () {

		if((time - timer)/time > 0.001f && timer > 0 ){
			//pbbackground.enabled = true;
			//pbforeground.enabled = true;
			float fracFull = (time - timer)/time;
			pbforeground.transform.position = new Vector3(foregroundPosReal.x - foregroundDimsReal.x/2 + foregroundDimsReal.x * fracFull / 2, 
			                                              foregroundPosReal.y, -1f);
			pbforeground.transform.localScale = new Vector3(foregroundDimsReal.x * fracFull, foregroundDimsReal.y);
			//pbforeground.color = color;
		}
		/*
		else{
			pbbackground.enabled = false;
			pbforeground.enabled = false;
			//pbbackground.transform.position = new Vector3(0, 2, -2);
			//powerbar.transform.position = new Vector3(0, 2, -1);
		}
		*/
	}

	public void StartTimer(float totalTime, Color c) {
		pbbackground.enabled = true;
		pbforeground.enabled = true;
		generatedTexture.SetPixel (0, 0, c);
		generatedTexture.Apply ();
		time = totalTime;
	}

	public void UpdateTimer(float currentTime) {
		timer = currentTime;
	}

	public void EndTimer() {
		pbbackground.enabled = false;
		pbforeground.enabled = false;
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







