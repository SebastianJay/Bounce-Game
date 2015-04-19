using UnityEngine;
using System.Collections;

/// <summary>
/// A class to handle setting up the environment after game-related events have occurred.
/// </summary>
public class InitialStateConfig : MonoBehaviour {

	//value assigned from inspector
	public Transform playerBallPrefab;

	//the player "switching out" process needs to happen on Awake to avoid race conditions and broken references
	void Awake() {
		//1 - pier
		if (Application.loadedLevel == 1) {
			if (DialogueConstantParser.EvaluateConstant("BobBodyGone")) {
				GameObject obj = GameObject.FindGameObjectWithTag("Player");
				if (obj != null) {
					Destroy(obj.transform.parent.gameObject);
					//Todo: determine starting position
					Instantiate(playerBallPrefab);
				}
			}
		}
	}
	
	void OnLevelWasLoaded(int level) {
		//to see which level maps to which int, look at the Build Settings
		//1 - pier
		if (level == 1)
		{
			if (DialogueConstantParser.EvaluateConstant("BobBodyGone")) {
				GameObject obj = GameObject.FindGameObjectWithTag("PierObjBody");
				obj.transform.GetChild(0).gameObject.SetActive(false);
				obj = GameObject.FindGameObjectWithTag("PierObjBall");
				obj.transform.GetChild(0).gameObject.SetActive(true);	//need child since finding inactive gameobject not possible
			}
		}
		//2 - city
		if (level == 2)
		{
			if (DialogueConstantParser.EvaluateConstant("FirstPlatformLowered")) {
				GameObject obj = GameObject.FindGameObjectWithTag("FirstPlatform");
				if (obj != null)
					obj.transform.position = obj.transform.position + new Vector3(0f, -15f, 0f);
			}
			if (DialogueConstantParser.EvaluateConstant("RiverGateOpen")) {
				GameObject obj = GameObject.FindGameObjectWithTag("RiverGate");
				if (obj != null)
					obj.transform.position = obj.transform.position + new Vector3(0f, 5f, 0f);
			}
			if (DialogueConstantParser.EvaluateConstant("RiverGate2Open")) {
				GameObject obj = GameObject.FindGameObjectWithTag("RiverGate2");
				if (obj != null)
					obj.transform.position = obj.transform.position + new Vector3(0f, -6.5f, 0f);
			}
			if (DialogueConstantParser.EvaluateConstant("WreckingBallLifted")) {
				GameObject obj = GameObject.FindGameObjectWithTag("WreckingBall");
				if (obj != null) {
					obj.transform.GetChild(0).position = obj.transform.GetChild(0).position + new Vector3(0f, 3.25f, 0f);
					obj.transform.GetChild(1).GetComponent<AnimatedChain>().CompleteAnimation();
				}
			}
			if (DialogueConstantParser.EvaluateConstant("AfterChildFound")) {
				GameObject obj = GameObject.FindGameObjectWithTag("MotherFollower");
				if (obj != null)
					obj.transform.position = new Vector3(451f, 1.8f, 0f);
			}
		}
	}
}
