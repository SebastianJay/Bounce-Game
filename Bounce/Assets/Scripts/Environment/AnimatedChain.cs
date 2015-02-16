using UnityEngine;
using System.Collections;

//special script for that chain link
public class AnimatedChain : MonoBehaviour {

	//initialized from inspector (in-order)
	public Transform[] chainLinks;
	public float animateTime = 3.0f;

	private float animateTimer = 0.0f;
	private bool isAnimating = false;
	private float contractDistance = 2.0f;
	private float originalDistance = 3.0f;

	// Use this for initialization
	void Start () {
			
	}
	
	// Update is called once per frame
	void Update () {
		if (isAnimating && animateTimer < animateTime)
		{
			animateTimer += Time.deltaTime;
		}
	}

	void Animate() {
		isAnimating = true;
	}
}
