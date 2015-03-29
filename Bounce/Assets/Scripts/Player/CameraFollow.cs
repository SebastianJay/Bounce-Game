using UnityEngine;
using System.Collections;

public struct CameraFollowConfig
{
	public Vector3 position;
	public Vector2 minXAndY;
	public Vector2 maxXAndY;
	public bool isLocked;
	public Vector2 lockedPosition;
	public float orthoSize;
}

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour 
{
	//arbitrary constant
	public const float camZCoordinate = -10.0f;

	//Vars for following the player
	public float xMargin = 1f;		// Distance in the x axis the player can move before the camera follows.
	public float yMargin = 1f;		// Distance in the y axis the player can move before the camera follows.
	public float xSmooth = 8f;		// How smoothly the camera catches up with it's target movement in the x axis.
	public float ySmooth = 8f;		// How smoothly the camera catches up with it's target movement in the y axis.
	public float orthoSmooth = 8f;		
	public Vector2 maxXAndY;		// The maximum x and y coordinates the camera can have.
	public Vector2 minXAndY;		// The minimum x and y coordinates the camera can have.

	//Vars for locking the camera in place
	public bool isLocked = false;	// The camera's position will not change if true.
	public float lockMoveTime = 3.0f;	//Time it takes to move to a locked position when activated
	public Vector2 lockedPosition = Vector2.zero; // the camera's new position
	public float lockedOrthoSize = 11f;	// The camera's orthographic size, for zooming in and out
	private float lockThreshold = 0.01f;

	private Transform player;		// Reference to the player's transform.
	private Camera cam;				// this object's camera component

	void Awake ()
	{
		// Setting up the reference.
		player = GameObject.FindGameObjectWithTag("Player").transform;
		cam = GetComponent<Camera> ();
	}

	public void LockCamera(Vector2 position, float orthoSize)
	{
		isLocked = true;
		lockedPosition = position;
		lockedOrthoSize = orthoSize;
	}

	public void UnlockCamera(Vector2 minVec, Vector2 maxVec, float orthoSize)
	{
		isLocked = false;
		if (minVec != Vector2.zero)
			minXAndY = minVec;
		if (maxVec != Vector2.zero)
			maxXAndY = maxVec;
		if (orthoSize != 0.0f)
			lockedOrthoSize = orthoSize;
	}

	void LateUpdate ()
	{
		if (player == null)
			player = GameObject.FindGameObjectWithTag ("Player").transform;
		if (!isLocked) {
			TrackPlayer ();
		} else {
			UpdateLockedPosition ();
		}
	}

	void TrackPlayer ()
	{
		// By default the target x and y coordinates of the camera are it's current x and y coordinates.
		float targetX = transform.position.x;
		float targetY = transform.position.y;
		float targetSize = cam.orthographicSize;

		if (transform.position.x < minXAndY.x && player.position.x < minXAndY.x)
			targetX = Mathf.Lerp(transform.position.x, minXAndY.x, xSmooth * Time.deltaTime);
		else if (transform.position.x > maxXAndY.x && player.position.x > maxXAndY.x)
			targetX = Mathf.Lerp(transform.position.x, maxXAndY.x, xSmooth * Time.deltaTime);
		// If the player has moved beyond the x margin...
		else if(Mathf.Abs(transform.position.x - player.position.x) > xMargin)
		{
			// ... the target x coordinate should be a Lerp between the camera's current x position and the player's current x position.
			targetX = Mathf.Lerp(transform.position.x, player.position.x, xSmooth * Time.deltaTime);
		}

		if (transform.position.y < minXAndY.y && player.position.y < minXAndY.y)
			targetY = Mathf.Lerp(transform.position.y, minXAndY.y, ySmooth * Time.deltaTime);
		else if (transform.position.y > maxXAndY.y && player.position.y > maxXAndY.y)
			targetY = Mathf.Lerp(transform.position.y, maxXAndY.y, ySmooth * Time.deltaTime);
		// If the player has moved beyond the y margin...
		else if(Mathf.Abs(transform.position.y - player.position.y) > yMargin)
		{
			// ... the target y coordinate should be a Lerp between the camera's current y position and the player's current y position.
			targetY = Mathf.Lerp(transform.position.y, player.position.y, ySmooth * Time.deltaTime);
		}

		targetSize = Mathf.Lerp(cam.orthographicSize, lockedOrthoSize, Time.deltaTime * orthoSmooth);

		// The target x and y coordinates should not be larger than the maximum or smaller than the minimum.
		//targetX = Mathf.Clamp(targetX, minXAndY.x, maxXAndY.x);
		//targetY = Mathf.Clamp(targetY, minXAndY.y, maxXAndY.y);

		// Set the camera's position to the target position with the same z component.
		transform.position = new Vector3(targetX, targetY, transform.position.z);
		cam.orthographicSize = targetSize;
	}

	void UpdateLockedPosition () 
	{
		float targetX = Mathf.Lerp(transform.position.x, lockedPosition.x, Time.deltaTime * xSmooth);		
		float targetY = Mathf.Lerp(transform.position.y, lockedPosition.y, Time.deltaTime * ySmooth);	
		float targetSize = Mathf.Lerp(cam.orthographicSize, lockedOrthoSize, Time.deltaTime * orthoSmooth);
		
		// Set the camera's position to the target position with the same z component.
		transform.position = new Vector3(targetX, targetY, transform.position.z);
		cam.orthographicSize = targetSize;
		if (Mathf.Abs (cam.orthographicSize - lockedOrthoSize) <= lockThreshold)
			cam.orthographicSize = lockedOrthoSize;
		if ((new Vector2(transform.position.x, transform.position.y) - lockedPosition).magnitude <= lockThreshold)
			transform.position = new Vector3(lockedPosition.x, lockedPosition.y, transform.position.z);
	}

	public CameraFollowConfig GetConfig()
	{
		CameraFollowConfig retVal = new CameraFollowConfig();
		retVal.position = transform.position;
		retVal.minXAndY = minXAndY;
		retVal.maxXAndY = maxXAndY;
		retVal.isLocked = isLocked;
		retVal.lockedPosition = lockedPosition;
		retVal.orthoSize = lockedOrthoSize;
		return retVal;
	}
	
	public void LoadConfig(CameraFollowConfig state, bool loadPosition = true)
	{
		if (loadPosition)
			transform.position = state.position;
		minXAndY = state.minXAndY;
		maxXAndY = state.maxXAndY;
		isLocked = state.isLocked;
		lockedPosition = state.lockedPosition;
		lockedOrthoSize = state.orthoSize;
	}
}
