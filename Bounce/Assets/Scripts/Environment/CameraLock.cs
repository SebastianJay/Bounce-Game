using UnityEngine;
using System.Collections;

public class CameraLock : MonoBehaviour
{
	public Vector2 cameraTarget;
	public float cameraOrthoSize;

	private GameObject camObj;

	void Start()
	{
		camObj = GameObject.FindGameObjectWithTag ("MainCamera");
	}

	void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.gameObject.tag == "Player") 
		{
			camObj.GetComponent<CameraFollow>().LockCamera(cameraTarget, cameraOrthoSize);
		}
	}
}

