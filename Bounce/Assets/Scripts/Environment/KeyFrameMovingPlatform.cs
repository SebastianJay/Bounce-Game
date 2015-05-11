using UnityEngine;
using System.Collections.Generic;

public class KeyFrameMovingPlatform : MovingPlatform {

	public List<Vector2> keyPoints;
	public float keySpeed = 3f;
	public bool travelToAndFro = false;
	public bool moveOnContact = true;

	private bool moving = false;
	private int keyIndex = 1;
	private bool reverseDir = false;
	private float keyTime;
	//moveTimer inherited
	//pauseTimer inherited
	//paused inherited

	// Use this for initialization
	void Start () {
		if (!moveOnContact)
			moving = true;
		if (useCurrentStartPosition)
		{
			keyPoints.Insert(0, transform.position);
		}
		keyTime = (keyPoints [1] - keyPoints [0]).magnitude / keySpeed;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (paused) {
			pauseTimer += Time.deltaTime;
			if (pauseTimer >= pauseTime)
			{
				pauseTimer = 0f;
				paused = false;
			}
		}
		else if (moving && (travelToAndFro || keyIndex < keyPoints.Count)) {
			int i = keyIndex;
			int j;
			if (reverseDir)
				j = keyIndex + 1;
			else
				j = keyIndex - 1;
			moveTimer += Time.deltaTime;
			Vector2 lerp = Vector2.Lerp(keyPoints[j], keyPoints[i], Mathf.Clamp(moveTimer/keyTime, 0.0f, 1.0f));
			if (moveParent)
				transform.parent.position = new Vector3(lerp.x,lerp.y,0f);
			else
				transform.position = new Vector3(lerp.x,lerp.y,0f);
			if (moveTimer >= keyTime) {
				if (!reverseDir)
					keyIndex++;
				else
					keyIndex--;
				if (travelToAndFro)
				{
					if (keyIndex == keyPoints.Count)
					{
						reverseDir = true;
						keyIndex = keyPoints.Count - 2;
						keyTime = (keyPoints [keyIndex] - keyPoints [keyIndex+1]).magnitude / keySpeed;
					}
					else if (keyIndex < 0)
					{
						reverseDir = false;
						keyIndex = 1;
						keyTime = (keyPoints [keyIndex] - keyPoints [keyIndex-1]).magnitude / keySpeed;
					}
					paused = true;
				}
				else if (keyIndex < keyPoints.Count) {
					keyTime = (keyPoints [keyIndex] - keyPoints [keyIndex-1]).magnitude / keySpeed;
					paused = true;
				}
				moveTimer = 0f;
			}
		}
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.tag == "Player" && moveOnContact)
		{
			moving = true;
		}
	}

	void OnCollisionEnter2D(Collision2D col) {
		if (col.gameObject.tag == "Player" && moveOnContact)
		{
			moving = true;
		}
	}

	public override void Reset()
	{
		base.Reset ();
		keyIndex = 1;
		reverseDir = false;
		keyTime = (keyPoints [1] - keyPoints [0]).magnitude / keySpeed;
		moving = !moveOnContact;
	}
}
