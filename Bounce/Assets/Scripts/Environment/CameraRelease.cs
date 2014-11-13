using UnityEngine;
using System.Collections;

public class CameraRelease : MonoBehaviour
{
	private GameObject camObj;
	public bool updateCameraRange = false;
	public bool updateCameraSize = false;
	public Vector2 minXAndY;
	public Vector2 maxXAndY;
	public float orthoSize;

	void Start()
	{
		camObj = GameObject.FindGameObjectWithTag ("MainCamera");
	}

	void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.gameObject.tag == "Player") 
		{
			Vector2 min = Vector2.zero;
			Vector2 max = Vector2.zero;
			float size = 0.0f;
			if (updateCameraRange)
			{
				min = minXAndY;
				max = maxXAndY;
			}
			if (updateCameraSize)
				size = orthoSize;
			camObj.GetComponent<CameraFollow>().UnlockCamera(min, max, size);
		}
	}
}

