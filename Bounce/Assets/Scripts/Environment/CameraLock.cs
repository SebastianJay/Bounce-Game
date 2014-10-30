using UnityEngine;
using System.Collections;

public class CameraLock : MonoBehaviour
{
	public Transform cameraTarget;

	void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.gameObject.tag == "Player") 
		{
			Debug.Log ("CameraLock triggered");
			GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>().isLocked = true;
			GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>().lockedPosition = cameraTarget;
		}
	}
}

