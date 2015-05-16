using UnityEngine;
using System.Collections;

/// <summary>
/// Script for a linearly moving platform. As currently implemented, should be attached
/// to an object with a TRIGGER collider attached as a child to the larger platform object
/// with a NON-TRIGGER collider. The trigger goes on the top part of the platform.
/// 
/// Can also be used for moving things which are not meant to be platformed on..
/// be sure to set moveParent to false then!!
/// </summary>
public class MovingPlatform : MonoBehaviour {
	public Vector2 initialPoint;
	public Vector2 endPoint;	//if flag below is set, this specifies an offset
	public bool useCurrentStartPosition = false;
	public float moveTime = 5.0f;	//in seconds, from start to end
	public float pauseTime = 2.0f;	//in seconds, time at endpoints
	public bool moveParent = true;	// if this collider is a "dummy child" to the larger platform
	public bool reparentPlayer = true;	//indicates whether player should be reparented to move with the platform 
	public bool flipOnTurn = false;	// whether object should rotate (to flip sprite) when changing direction
	[HideInInspector]
	public bool hasOneWayParent = false;

	protected float moveTimer = 0.0f;
	protected float pauseTimer = 0.0f;
	protected bool paused = false;
	private Vector2 mFrom;
	private Vector2 mTo;

	void Start() {
		Reset ();
		if (moveParent && transform.parent.GetComponent<OneWay>() != null)
			hasOneWayParent = true;
	}

	void FixedUpdate () {
		if (paused)
		{
			pauseTimer += Time.deltaTime;
			if (pauseTimer >= pauseTime)
			{
				if (flipOnTurn) {
					if (moveParent) {
						//if player is attached to this platform we don't want him to rotate suddenly
						Transform playerContainer=null;
						if (transform.parent.childCount > 1) {
							playerContainer = transform.parent.GetChild(1);
							playerContainer.parent = null;
						}
						transform.parent.RotateAround(transform.parent.position, new Vector3(0f, 1f, 0f), 180f);
						if (playerContainer != null)
							playerContainer.parent = transform.parent;
					}
					else
						transform.RotateAround(transform.position, new Vector3(0f, 1f, 0f), 180f);
				}
				pauseTimer = 0.0f;
				paused = false;
			}
		}
		else
		{
			moveTimer += Time.deltaTime;
			if (moveTimer >= moveTime)
			{
				moveTimer = 0.0f;
				Vector2 temp = mFrom;
				mFrom = mTo;
				mTo = temp;
				paused = true;
			}
			float frac = moveTimer / moveTime;
			//rigidbody2D.MovePosition (Vector2.Lerp (mFrom, mTo, Mathf.Clamp (frac, 0.0f, 1.0f)));
			Vector2 lerp = Vector2.Lerp(mFrom, mTo, Mathf.Clamp(frac, 0.0f, 1.0f));
			if (moveParent)
				transform.parent.position = new Vector3(lerp.x,lerp.y,0f);
			else
				transform.position = new Vector3(lerp.x,lerp.y,0f);
		}
	}

	public virtual void Reset()
	{
		if (useCurrentStartPosition)
		{
			if (moveParent)
				mFrom = transform.parent.position;
			else
				mFrom = transform.position;
			mTo = mFrom+endPoint;			
		}
		else
		{
			transform.position = new Vector3(initialPoint.x, initialPoint.y, 0);
			mFrom = initialPoint;
			mTo = endPoint;
		}
		moveTimer = 0f;
		pauseTimer = 0f;
		paused = false;
	}
}
