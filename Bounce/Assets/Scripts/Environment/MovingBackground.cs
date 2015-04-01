using UnityEngine;
using System.Collections;

//for stuff along x-axis, though it can easily be adapted for y
public class MovingBackground : MonoBehaviour {

	public bool randomBetweenConstants = true;
	//speed can be in absolute value and sign will be taken care of by startX, endX
	public float floorSpeed = 1f;
	public float ceilSpeed = 2f;
	public float startX = -1e5f;
	public float destX = 1e5f;

	private float mVelocity;
	private float mSign;

	// Use this for initialization
	void Start () {
		mSign = Mathf.Sign(destX - startX);
		InitSpeed();
	}
	
	// Update is called once per frame
	void Update () {
		if (mSign < 0 && transform.position.x < destX) {
			transform.position = new Vector3(startX, transform.position.y, 0f);
			InitSpeed();
		}
		else if (mSign > 0 && transform.position.x > destX) {
			transform.position = new Vector3(startX, transform.position.y, 0f);
			InitSpeed();
		}
		transform.position = transform.position + new Vector3(mVelocity * Time.deltaTime, 0f, 0f);
	}

	void InitSpeed() {
		if (randomBetweenConstants) {
			mVelocity = Random.Range(floorSpeed, ceilSpeed) * mSign;
		} 
		else
			mVelocity = floorSpeed * mSign;
	}
}
