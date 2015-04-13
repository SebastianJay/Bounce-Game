using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class DialogueConstantParser
{
	public static HashSet<string> constantSet = new HashSet<string>();
	public static bool eventLock = false;

	public static void ExecuteEvent(string eventName)
	{
		GameObject obj = null;
		//We hard-code every possible dialogue event here!
		//Typically we accomplish this by picking out GameObjects using tags
		//	and modifying, enabling, or invoking a method in some component of theirs
		switch(eventName)
		{
		case "MotherFollows":
			obj = GameObject.FindGameObjectWithTag("MotherFollower");
			if (obj != null)
				obj.GetComponent<FollowAI>().enabled = true;
			break;
		case "LiftRiverGate":
			obj = GameObject.FindGameObjectWithTag("RiverGate");
			if (obj != null)
				obj.GetComponent<AnimatedMover>().MoveRelative(new Vector3(0f, 5f, 0f), 2.0f);
			SetConstant("RiverGateOpen");
			break;
		case "ChildFound":
			if (!EvaluateConstant("OnChildFound")) {
				obj = GameObject.FindGameObjectWithTag("MotherFollower");
				SetConstant("OnChildFound");
				if (obj != null) {
					obj.GetComponent<FollowAI>().enabled = false;
					//we force a conversation to happen here
					obj.transform.GetChild(0).GetComponent<Interactable>().StepConvo();
				}
			}
			break;
		case "MoveMotherToChild":
			obj = GameObject.FindGameObjectWithTag("MotherFollower");
			if (obj != null) {
				obj.GetComponent<AnimatedMover>().MoveAbsolute(new Vector3(451f, 1.8f, 0f), 
				                                               (new Vector3(451f, 1.8f, 0f)-obj.transform.position).magnitude / 15f);
				GameObject.FindGameObjectWithTag("DialogueSystem").GetComponent<DialogueSystem>().ReparentNPCText(obj.transform);
			}
			obj = GameObject.FindGameObjectWithTag("MainCamera");
			if (obj != null) {
				//recenter camera
				obj.GetComponent<CameraFollow>().lockedPosition = new Vector2(451f, 2.1f);
			}
			break;
		case "BobSeparated":
			eventLock = true;
			obj = GameObject.FindGameObjectWithTag("Player");
			Vector3 playerPos = Vector3.zero;
			if (obj != null) {
				obj.transform.GetChild(0).parent = null;
				//obj.rigidbody2D.fixedAngle = false;
				obj.rigidbody2D.AddForce(new Vector2(-3500f, 5000f));
				obj.collider2D.enabled = false;
				obj.GetComponent<PlayerBodyControl>().enabled = false;
				playerPos = obj.transform.position;
			}
			obj = GameObject.FindGameObjectWithTag("Villain");
			if (obj != null) {
				//inject animation
				obj.transform.GetChild(1).position = playerPos;
				obj.transform.GetChild(1).gameObject.SetActive(true);
				//inject sound
				obj.transform.GetChild(2).audio.Play();
			}
			//move camera out
			obj = GameObject.FindGameObjectWithTag("MainCamera");
			if (obj != null) {
				obj.GetComponent<CameraFollow>().isLocked = true;
				obj.GetComponent<CameraFollow>().lockedOrthoSize = obj.GetComponent<Camera>().orthographicSize;
				obj.GetComponent<CameraFollow>().lockedPosition = obj.transform.position;
				//queue reload of level
				//...probably shouldn't do it this way (grabbing a random component and calling coroutine)
				obj.GetComponent<CameraFollow>().StartCoroutine(RobScenePan());
			}
			break;
		case "FreezeControls":
			obj = GameObject.FindGameObjectWithTag("Player");
			if (obj.GetComponent<PlayerBallControl>() != null)
				obj.GetComponent<PlayerBallControl>().playerLock = true;
			else if (obj.GetComponent<PlayerBodyControl>() != null)
				obj.GetComponent<PlayerBodyControl>().playerLock = true;
			eventLock = true;
			break;
		case "UnFreezeControls":
			obj = GameObject.FindGameObjectWithTag("Player");
			if (obj.GetComponent<PlayerBallControl>() != null)
				obj.GetComponent<PlayerBallControl>().playerLock = false;
			else if (obj.GetComponent<PlayerBodyControl>() != null)
				obj.GetComponent<PlayerBodyControl>().playerLock = false;
			eventLock = false;
			break;
		}

	}

	/// <summary>
	/// Checks if the string is in our list of defined constants.
	/// If not, performs hard-coded checks to see if the constant is valid.
	/// </summary>
	/// <param name="constant">Some string value that stands for a unique constant</param>
	public static bool EvaluateConstant(string constant)
	{
		// first check our set
		if (constantSet.Contains(constant))
			return true;

		// for string literals in the dialogue
		// which cannot be found in the hashset
		switch(constant)
		{
		case "MotherIsFollowing":
			GameObject obj = GameObject.FindGameObjectWithTag("MotherFollower");
			if (obj != null)
				return obj.GetComponent<FollowAI>().enabled;
			break;
		}

		return false;
	}

	public static void SetConstant(string constant)
	{
		constantSet.Add(constant);
	}

	//Event Specific methods below
	/*
	 * RobScenePan - there's a pause after the head is blown off and before we zoom in to Rob
	 */
	static IEnumerator RobScenePan() {
		yield return new WaitForSeconds(2.0f);

		GameObject obj = GameObject.FindGameObjectWithTag("Villain");
		obj.transform.GetChild (3).audio.Play ();

		GameObject obj2 = GameObject.FindGameObjectWithTag ("MainCamera");
		obj2.GetComponent<CameraFollow>().isLocked = true;
		obj2.GetComponent<CameraFollow>().lockedOrthoSize = 5f;
		obj2.GetComponent<CameraFollow>().lockedPosition = new Vector2(obj.transform.position.x, obj.transform.position.y);
		
		GameObject obj3 = GameObject.FindGameObjectWithTag ("ScreenFader");
		obj3.GetComponent<ScreenFading>().fadeSpeed = 1f;	//to temporarily make the fade longer
		obj3.GetComponent<ScreenFading>().Transition (RobSceneReload, true);
	}
	/*
	 * RobSceneReload - the screen goes to black as the Pier level reloads with Bob without his body
	 */
	static void RobSceneReload() {
		DialogueConstantParser.SetConstant("BobBodyGone");
		eventLock = false;
		Application.LoadLevel("Pier");
	}
}
