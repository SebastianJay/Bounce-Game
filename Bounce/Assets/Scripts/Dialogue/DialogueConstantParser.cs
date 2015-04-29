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
		//hopefully not a great performance cost doing it this way..
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
				//obj.GetComponent<CameraFollow>().lockedOrthoSize = obj.GetComponent<Camera>().orthographicSize;
				obj.GetComponent<CameraFollow>().lockedOrthoSize = 11f;
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
		case "DoMonologue":
			if (!EvaluateConstant("MonologueDone")) {
				obj = GameObject.FindGameObjectWithTag("Monologue");
				obj.transform.GetComponent<Interactable>().StepConvo();
				obj.transform.GetComponent<Interactable>().StartCoroutine(GiveTalkProceedInstructions());
			}
			break;
		case "TalkToCaptain":
			bool talkFlag = false;
			if (!EvaluateConstant("FirstCaptainTalkDone")) {
				SetConstant("InitialCaptainTalkDone");
				talkFlag = true;
			}
			else if (EvaluateConstant("BobBodyGone") && !EvaluateConstant("SecondCaptainTalkDone")) {
				SetConstant("SecondCaptainTalkDone");
				talkFlag = true;
			}
			if (talkFlag) {
				obj = GameObject.FindGameObjectWithTag("Captain");
				obj.transform.GetComponent<Interactable>().StepConvo();
			}
			break;
		case "BobbityGivesBobGlasses":
			if(!EvaluateConstant ("GivenGlassesFromBobbity")){
				SetConstant ("GivenGlassesFromBobbity");

				GameObject notifyObj = GameObject.FindGameObjectWithTag ("NoteManager");
				obj = GameObject.FindGameObjectWithTag("Parrot");
				obj.audio.Play ();
				ItemType type = ItemType.Aviator;
				PlayerDataManager.inventory.AddItem(type);
				if (notifyObj != null) {
					notifyObj.GetComponent<NotificationManager>().PushMessage(
						"Added the item \"" + ImmutableData.GetItemData()[type].name + "\" to Inventory");
				}
			}
			break;
		case "BobbitySqauwk":
			//play parrot sqauwk
			break;
		case "BobRespawnsAndTalksToKing":
			// REALLY BROKEN.. considering refactoring later
			obj = GameObject.FindGameObjectWithTag("ScreenFader");
			//obj.GetComponent<PlayerBallControl>().playerLock = true;
			//obj.GetComponent<PlayerBallControl>().StartCoroutine(BobTalksToKingAnimation());
			obj.GetComponent<ScreenFading>().StartCoroutine(BobTalksToKingAnimation());
			break;
		case "ActivateTeleporter":
			obj = GameObject.FindGameObjectWithTag("TeleportMachine");
			obj.transform.GetChild(0).gameObject.SetActive(true);
			obj.transform.GetChild(1).gameObject.SetActive(true);
			SetConstant("TeleporterActive");
			break;
		case "TeleportAnimation":
			obj = GameObject.FindGameObjectWithTag("Player");
			DialogueConstantParser.eventLock = true;
			obj.GetComponent<PlayerBallControl>().playerLock = true;
			obj.rigidbody2D.velocity = Vector2.zero;
			obj.rigidbody2D.angularVelocity = 0f;
			obj = GameObject.FindGameObjectWithTag("ScreenFader");
			obj.GetComponent<ScreenFading>().StartCoroutine(AnimateTeleporter());
			break;
		case "GiveTalkInstructions":
			if (!EvaluateConstant("TalkInstructionsDone")) {
				obj = GameObject.FindGameObjectWithTag("NoteManager");
				obj.transform.GetComponent<NotificationManager>().PushMessage("Use 'W'/'S' (or Up/Down) to choose responses.", 10f);
				obj.transform.GetComponent<NotificationManager>().PushMessage("Press 'E' (or 'R Ctrl') to talk.", 10f);
				SetConstant("TalkInstructionsDone");
			}
			break;
		case "GiveWalkInstructions":
			if (!EvaluateConstant("WalkInstructionsDone")) {
				obj = GameObject.FindGameObjectWithTag("NoteManager");
				obj.transform.GetComponent<NotificationManager>().PushMessage("Press 'A'/'D' (or Left/Right) to walk.", 10f);
				SetConstant("WalkInstructionsDone");
			}
			break;
		case "GiveJumpInstructions":
			if (!EvaluateConstant("JumpInstructionsDone")) {
				obj = GameObject.FindGameObjectWithTag("NoteManager");
				obj.transform.GetComponent<NotificationManager>().PushMessage("Press 'W' (or Up) to jump.", 10f);
				SetConstant("JumpInstructionsDone");
			}
			break;
		case "GiveMenuOpenInstructions":
			if (!EvaluateConstant("MenuOpenInstructionsDone")) {
				obj = GameObject.FindGameObjectWithTag("NoteManager");
				obj.transform.GetComponent<NotificationManager>().PushMessage("Press 'Q' (or R Shift) to toggle the menu.", 10f);
				SetConstant("MenuOpenInstructionsDone");
			}
			break;
		case "GiveBounceBoostInstructions":
			if (!EvaluateConstant("BounceBoostInstructionsDone")) {
				obj = GameObject.FindGameObjectWithTag("NoteManager");
				obj.transform.GetComponent<NotificationManager>().PushMessage("Hold 'W' (or Up) when hitting the ground to boost back up.", 10f);
				SetConstant("BounceBoostInstructionsDone");
			}
			break;
		case "GiveBounceDepressInstructions":
			if (!EvaluateConstant("BounceDepressInstructionsDone")) {
				obj = GameObject.FindGameObjectWithTag("NoteManager");
				obj.transform.GetComponent<NotificationManager>().PushMessage("Hold 'S' (or Down) when hitting the ground to stop your bounce.", 10f);
				SetConstant("BounceDepressInstructionsDone");
			}
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

	static IEnumerator GiveTalkProceedInstructions() {
		yield return new WaitForSeconds (1.5f);
		GameObject obj = GameObject.FindGameObjectWithTag("NoteManager");
		obj.GetComponent<NotificationManager>().PushMessage ("Press 'E' (or 'R Ctrl') to continue talking.", 10f);
	}

	static IEnumerator BobTalksToKingAnimation() {
		yield return new WaitForSeconds(0.02f);
		GameObject obj = GameObject.FindGameObjectWithTag("Player");
		GameObject obj2 = GameObject.FindGameObjectWithTag("KingRobert");
		obj.GetComponent<PlayerBallControl>().playerLock = true;
		obj.rigidbody2D.AddForce (new Vector2 (-615f, 0f));
		obj.rigidbody2D.AddTorque (200f);
		yield return new WaitForSeconds (7.0f);
		obj2.GetComponent<Interactable>().StepConvo ();
	}

	static IEnumerator AnimateTeleporter() {
		GameObject obj = GameObject.FindGameObjectWithTag("TeleportMachine");
		Transform particleObj = obj.transform.GetChild (1);
		for (int i = 0; i < 40; i++)
		{
			particleObj.particleSystem.emissionRate += 2f;
			yield return new WaitForSeconds(0.05f);
		}
		particleObj.particleSystem.emissionRate = 0f;
		Transform particleObj2 = obj.transform.GetChild (2);
		particleObj2.gameObject.SetActive (true);
		obj.audio.Play();
		GameObject playerObj = GameObject.FindGameObjectWithTag ("Player");
		playerObj.SetActive (false);	//?
		yield return new WaitForSeconds(1f);
		GameObject faderObj = GameObject.FindGameObjectWithTag ("ScreenFader");
		faderObj.GetComponent<ScreenFading>().fadeSpeed = 1f;	//to temporarily make the fade longer
		faderObj.GetComponent<ScreenFading>().Transition(delegate {
			DialogueConstantParser.eventLock = false;
			PlayerDataManager.loadedLevel = false;
			Application.LoadLevel("Space");
		}, true);
	}
}
