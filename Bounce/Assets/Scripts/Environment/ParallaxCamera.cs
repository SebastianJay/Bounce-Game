using UnityEngine;
using System.Collections;
using System;

public delegate void ParaCamHandler(object sender, ParaCamArgs e);

public class ParaCamArgs : EventArgs {
	public float dx;
	public float dy;
	public float orthoSize;
}

public class ParallaxCamera : MonoBehaviour {

	public event ParaCamHandler ChangeHandler;
	public float epsilon = 0.0001f;	//threshold for sending an event
	private Camera camComponent;
	private float mX;
	private float mY;

	// Use this for initialization
	void Start () {
		mX = transform.position.x;
		mY = transform.position.y;
		camComponent = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
		float x = transform.position.x;
		float y = transform.position.y;
		if ((Mathf.Abs(x - mX) > epsilon 
		 	|| Mathf.Abs(y - mY) > epsilon)
		    && ChangeHandler != null)
		{
			ParaCamArgs e = new ParaCamArgs();
			e.dx = x - mX;
			e.dy = y - mY;
			e.orthoSize = camComponent.orthographicSize;
			ChangeHandler(this, e);
		}
		mX = x;
		mY = y;
	}
}
