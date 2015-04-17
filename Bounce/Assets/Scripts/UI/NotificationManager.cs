using UnityEngine;
using System.Collections.Generic;

public class NotificationManager : MonoBehaviour {

	public Transform messagePrefab;
	public float newMessageOffset = 0.05f;
	private List<Transform> messages = new List<Transform>();
	
	public void PushMessage(string message, float showTime = 3f)
	{
		Transform obj = Instantiate (messagePrefab) as Transform;
		obj.GetComponent<GUIText> ().text = message;
		obj.GetComponent<NotificationMessage> ().showTime = showTime;
		obj.parent = transform;
		//clean up list to remove messages that have deleted themselves
		messages.RemoveAll (msg => msg == null);
		//shift current messages downward
		foreach (Transform msg in messages) {
			msg.position = new Vector3(msg.position.x, msg.position.y - newMessageOffset, 0f);
		}
		messages.Add (obj);
	}

	/*
	void Update() {
		if (Input.GetKeyDown(KeyCode.P)) {
			PushMessage("Test Message. Hello!", 4f);
		}
	}
	*/
}
