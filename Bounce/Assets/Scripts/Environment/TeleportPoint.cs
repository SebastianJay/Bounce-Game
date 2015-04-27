using UnityEngine;
using System.Collections;

public class TeleportPoint : MonoBehaviour {

	//set from inspector
	public TeleportPoint otherPoint;
	public bool teleportedTo;
	public bool killVelocity;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.tag == "Player" && !teleportedTo && otherPoint != null) {
			col.transform.position = otherPoint.transform.position;
			otherPoint.teleportedTo = true;
			teleportedTo = false;

			if (killVelocity)
			{
				col.rigidbody2D.velocity = Vector2.zero;
			}

			if (transform.audio != null)
			{
				transform.audio.Play ();
			}
		}
	}
	
	void OnTriggerStay2D(Collider2D col)
	{
		if (col.tag == "Player") {
		}
	}

	void OnTriggerExit2D(Collider2D col)
	{
		if (col.tag == "Player") {
			teleportedTo = false;
		}
	}
}
