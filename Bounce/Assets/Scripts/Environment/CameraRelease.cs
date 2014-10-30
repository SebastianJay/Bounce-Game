using UnityEngine;
using System.Collections;

public class CameraRelease : MonoBehaviour
{

	void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.gameObject.tag == "Player") 
		{
			Debug.Log ("CameraRelease triggered");
			GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>().isLocked = false;
			GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>().lockedPosition = null;
		}
	}
}

