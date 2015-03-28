using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class DialogueConstantParser
{
	public static HashSet<string> constantSet = new HashSet<string>();

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
}
