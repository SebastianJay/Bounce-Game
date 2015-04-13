using UnityEngine;
using System.Collections;

//special script for that chain link attached to wrecking ball
//very hastily put together since it'll only be used once >_>
public class AnimatedChain : MonoBehaviour {

	//initialized from inspector (in-order)
	public Transform[] chainLinks;
	public float animateTime = 3.0f;
	public float contractDistance = 2.0f;

	private float animateTimer = 0.0f;
	private bool isAnimating = false;
	private float originalDistance = 3.0f;
	private float originalPos = 0.0f;
	private BoxCollider2D col;
	// Use this for initialization
	void Start () {
		originalPos = chainLinks [0].position.y;
		originalDistance = Mathf.Abs(chainLinks [chainLinks.Length - 1].position.y - chainLinks [0].position.y);
		col = GetComponent<BoxCollider2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (isAnimating && animateTimer < animateTime)
		{
			animateTimer += Time.deltaTime;
			if (animateTimer >= animateTime) {
				animateTimer = animateTime;	//cap it
				isAnimating = false;
			}
			float dist = originalDistance - (originalDistance - contractDistance) * animateTimer / animateTime;
			for (int i = 0; i < chainLinks.Length; i++)
			{
				chainLinks[i].position = new Vector3(chainLinks[i].position.x,
				                                     originalPos - dist * (i * 1.0f / (chainLinks.Length-1)),
				                      				 chainLinks[i].position.z);
			}
			col.size = new Vector2(col.size.x, dist);
			col.center = new Vector2(col.center.x, animateTimer/animateTime * (dist/2));
		}
		//debug
		//if (Input.GetKeyDown (KeyCode.A))
		//	Animate ();
	}

	public void CompleteAnimation() {
		isAnimating = true;
		animateTimer = animateTime - 0.001f;
	}

	public void Animate(float time) {
		animateTime = time;
		isAnimating = true;
	}
}
