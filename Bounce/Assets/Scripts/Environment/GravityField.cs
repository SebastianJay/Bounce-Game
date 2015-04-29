using UnityEngine;
using System.Collections;

public class GravityField : MonoBehaviour {

	public bool directional = false;	//if true, gravity points one direction
										//if false, gravity points to center of this object (like planet)
	public Vector2 direction;

	public float gravityStrength = 30f;
	public float additionalForce = 30f;
	public float appliedDrag = 2f;

	public static Transform dominantField;
	private GameObject playerObj;

	// Use this for initialization
	void Start () {
		//playerObj = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
		if (dominantField == transform) {
			if (!directional) {
				Vector2 diff = new Vector2(transform.position.x - playerObj.transform.position.x,
				                           transform.position.y - playerObj.transform.position.y);
				Physics2D.gravity = diff.normalized * gravityStrength;
				playerObj.rigidbody2D.AddForce(Physics2D.gravity.normalized * additionalForce * diff.sqrMagnitude);
			} else {
				playerObj.rigidbody2D.AddForce(direction.normalized * additionalForce);
			}
		}
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.tag == "Player") {
			playerObj = col.gameObject;
			dominantField = transform;
			if (!directional) {
				Vector2 diff = new Vector2(transform.position.x - col.transform.position.x,
				                           transform.position.y - col.transform.position.y);
				Physics2D.gravity = diff.normalized * gravityStrength;
				col.rigidbody2D.drag = appliedDrag;
			} else {
				Physics2D.gravity = direction.normalized * gravityStrength;
			}
		}
	}

	/*
	void OnTriggerStay2D(Collider2D col)
	{
	}
	*/

}
