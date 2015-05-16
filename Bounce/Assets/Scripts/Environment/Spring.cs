using UnityEngine;
using System.Collections;

public class Spring : MonoBehaviour {
	public float springForce = 4000f;
	public AudioClip contactNoise;
	public float noiseVolume = 1f;

	public const int springJumpFrameThreshold = 10;
	public const float springJumpTimeThreshold = 0.2f;

	private float orientation;
	private Vector2 direction;
	private AudioSource noiseSrc;
	private GameObject screenFadeObj;

	void Awake() {
		orientation = (this.gameObject.transform.rotation.eulerAngles.z);
		direction = new Vector2(- Mathf.Sin(orientation * Mathf.PI / 180), Mathf.Cos(orientation * Mathf.PI / 180));
		if (contactNoise != null){
			noiseSrc = gameObject.AddComponent<AudioSource> ();
			noiseSrc.clip = contactNoise;
			noiseSrc.volume = noiseVolume;
		}
		screenFadeObj = GameObject.FindGameObjectWithTag("ScreenFader");
	}

	/*
	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "Player")
		{
			if (!coll.gameObject.GetComponent<PlayerBallControl>().jumpedInCurrentFrame
			    && !coll.gameObject.GetComponent<PlayerBallControl>().springInCurrentFrame)
			{
				coll.gameObject.rigidbody2D.AddForce (springForce * direction);
				coll.gameObject.GetComponent<PlayerBallControl>().springInCurrentFrame = true;
				Debug.Log ("Spring bounce");
				if (noiseSrc != null)
					noiseSrc.Play();
			}
		}
	}
	*/

	public void SpringCollide(GameObject player) {
		if (Time.timeSinceLevelLoad - player.GetComponent<PlayerBallControl>().jumpLastTime > springJumpTimeThreshold
		    && Time.timeSinceLevelLoad - player.GetComponent<PlayerBallControl>().springLastTime > springJumpTimeThreshold)
		{
			
			player.rigidbody2D.AddForce (springForce * direction.normalized);
			player.GetComponent<PlayerBallControl>().springLastTime = Time.timeSinceLevelLoad;
			//Debug.Log ("Spring bounce " + Time.frameCount);
			if (noiseSrc != null && (screenFadeObj == null || !screenFadeObj.GetComponent<ScreenFading>().IsTransitioning()))
				noiseSrc.Play();
		}
	}

}