using UnityEngine;
using System.Collections;

/// <summary>
/// Same design strategy as MovingPlatform, but this one is for sinusoidal movement.
/// The values entered into the parent are ignored.
/// </summary>
public class SinMovingPlatform : MovingPlatform {
	public bool xAxis = true;
	public bool yAxis = false;
	public float amplitude = 1.0f;

	private Vector3 center;
	private float frequency;

	// Use this for initialization
	void Start () {
		center = transform.position;
		frequency = 2*Mathf.PI / moveTime;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		moveTimer += Time.deltaTime;
		float xcomp;
		float ycomp;
		if (moveParent) {
			xcomp = transform.parent.position.x;
			ycomp = transform.parent.position.y;
		} else {
			xcomp = transform.position.x;
			ycomp = transform.position.y;
		}
		if (xAxis) {
			xcomp = center.x + amplitude * Mathf.Cos(frequency * moveTimer); 
		}
		if (yAxis) {
			ycomp = center.y + amplitude * Mathf.Sin(frequency * moveTimer); 
		}
		if (moveParent)
			transform.parent.position = new Vector3(xcomp, ycomp, 0f);
		else
			transform.position = new Vector3(xcomp, ycomp, 0f);
	}
}
