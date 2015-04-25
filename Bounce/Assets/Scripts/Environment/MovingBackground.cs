using UnityEngine;
using System.Collections;

public class MovingBackground : MonoBehaviour {

	public bool randomBetweenConstants = true;
	//speed can be in absolute value and sign will be taken care of by startX, endX
	public float floorSpeed = 1f;
	public float ceilSpeed = 2f;
	public bool xaxis = true;
	public float startX = -1e5f;
	public float destX = 1e5f;
	public bool yaxis = false;
	public float startY = -1e5f;
	public float destY = 1e5f;

	private float mVelocityX;
	private float mVelocityY;
	private float mSignX;
	private float mSignY;
	//
	// Use this for initialization
	void Start () {
		mSignX = Mathf.Sign(destX - startX);
		mSignY = Mathf.Sign(destY - startY);
		InitSpeed();
	}
	
	// Update is called once per frame
	void Update () {
		if ((mSignX < 0 && transform.position.x < destX) || (mSignX > 0 && transform.position.x > destX)) {
			transform.position = new Vector3(startX, transform.position.y, 0f);
			InitSpeed();
		}
		if ((mSignY < 0 && transform.position.y < destY) || (mSignY > 0 && transform.position.y > destY)) {
			transform.position = new Vector3(transform.position.x, startY, 0f);
			InitSpeed();
		}
		float targetX, targetY;
		if (xaxis)
			targetX = mVelocityX * Time.deltaTime;
		else 
			targetX  = 0f;
		if (yaxis)
			targetY = mVelocityY * Time.deltaTime;
		else 
			targetY = 0f;

		transform.position = transform.position + new Vector3 (targetX, targetY, 0f);
	}

	void InitSpeed() {
		if (randomBetweenConstants) {
			mVelocityX = Random.Range(floorSpeed, ceilSpeed) * mSignX;
			mVelocityY = Random.Range(floorSpeed, ceilSpeed) * mSignY;
		} 
		else {
			mVelocityX = floorSpeed * mSignX;
			mVelocityY = floorSpeed * mSignY;
		}
	}
}
