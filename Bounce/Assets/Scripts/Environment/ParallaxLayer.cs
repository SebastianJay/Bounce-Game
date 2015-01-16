using UnityEngine;
using System.Collections;

public class ParallaxLayer : MonoBehaviour {

	public bool moveHorizontally = true;
	public bool moveVertically = false;
	public float horizontalFactor = 1.0f;
	public float verticalFactor = 1.0f;

	// Use this for initialization
	void Start () {
		GameObject cam = GameObject.FindGameObjectWithTag ("MainCamera");
		cam.GetComponent<ParallaxCamera>().ChangeHandler += new ParaCamHandler (UpdateParallax);
	}

	private void UpdateParallax(object sender, ParaCamArgs e) {
		float zfactor = 7f / e.orthoSize;	//kinda arbitrary, but parallax should be less if camera is zoomed out
		if (moveHorizontally)
		{
			transform.position = transform.position - new Vector3(e.dx * horizontalFactor * zfactor, 0f, 0f);
		}
		if (moveVertically)
		{
			transform.position = transform.position - new Vector3(0f, e.dy * verticalFactor * zfactor, 0f);
		}
	}
}
