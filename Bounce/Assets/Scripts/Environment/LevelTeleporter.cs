using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class LevelTeleporter : MonoBehaviour {

	public string levelToTeleportTo;
	public int targetID = 0;
	public static Dictionary<int,Vector3> teleporterTargetTable = new Dictionary<int, Vector3>();
	public static bool teleported = false;
	public static int teleportTarget = 0;

	private GameObject screenFadeObj;
	private bool locked = false;

	void Start()
	{
		screenFadeObj = GameObject.FindGameObjectWithTag ("ScreenFader");
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player" && !locked) {
			other.GetComponent<PlayerDataManager>().saveCurrent();

			if (screenFadeObj != null)
			{
				locked = true;
				screenFadeObj.GetComponent<ScreenFading>().Transition(TeleportTransition, true);
			}
			else
				TeleportTransition();
		}
	}

	void TeleportTransition()
	{
		teleported = true;
		teleportTarget = targetID;
		Application.LoadLevel(levelToTeleportTo);
		locked = false;
	}

}
