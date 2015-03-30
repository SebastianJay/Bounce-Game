using UnityEngine;
using System.Collections;

public class CannonLaunch : MonoBehaviour {

	public float cannonForce = 20000f;
	public string levelToTeleportTo;
	public int targetID = 0;

	private GameObject screenFadeObj;
	private GameObject playerObj;
	private bool locked = false;
	
	// Use this for initialization
	void Start () {
		screenFadeObj = GameObject.FindGameObjectWithTag ("ScreenFader");
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player" && other.GetComponent<PlayerBallControl>() != null && !locked) {

			playerObj = other.gameObject;
			other.rigidbody2D.velocity = Vector2.zero;
			other.rigidbody2D.angularVelocity = 0f;
			other.rigidbody2D.gravityScale = 0f;
			other.GetComponent<PlayerBallControl>().playerLock = true;
			DialogueConstantParser.eventLock = true;
			GetComponent<EdgeCollider2D>().enabled = false;

			GameObject obj2 = GameObject.FindGameObjectWithTag ("MainCamera");
			obj2.GetComponent<CameraFollow>().isLocked = true;
			obj2.GetComponent<CameraFollow>().lockedOrthoSize = 13f;
			obj2.GetComponent<CameraFollow>().lockedPosition = new Vector2(obj2.transform.position.x, obj2.transform.position.y);

			StartCoroutine(TeleportEvent());
		}
	}

	IEnumerator TeleportEvent()
	{
		yield return new WaitForSeconds (2f);

		//push force onto player
		Vector3 delta = transform.GetChild(0).position - playerObj.transform.position;
		Vector2 unitdelta = new Vector2(delta.x, delta.y).normalized;
		playerObj.rigidbody2D.gravityScale = 1f;
		playerObj.rigidbody2D.AddForce(unitdelta * cannonForce);
		playerObj.collider2D.enabled = false;
		//make explosion sound
		//transform.audio.Play ();
		//launch "fireworks"
		transform.GetChild(0).gameObject.SetActive (true);

		yield return new WaitForSeconds (2f);
		if (screenFadeObj != null)
		{
			locked = true;
			screenFadeObj.GetComponent<ScreenFading>().Transition(TeleportTransition, true);
		}
		else
			TeleportTransition();
	}

	void TeleportTransition()
	{
		TeleporterTarget.teleportTarget = targetID;
		Application.LoadLevel(levelToTeleportTo);
		DialogueConstantParser.eventLock = false;
		PlayerDataManager.loadedLevel = false;
		locked = false;
	}
}
