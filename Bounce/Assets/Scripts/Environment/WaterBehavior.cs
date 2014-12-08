using UnityEngine;
using System.Collections;

public class WaterBehavior : MonoBehaviour {
	public AudioClip bigSplashNoise;
	public AudioClip smallSplashNoise;
	public float bigSplashVolume = 1f;
	public float smallSplashVolume = 1f;
	public float bigNoiseThreshold = 5f;
	public float noNoiseThreshold = 0.5f;

	private Vector2 waterForce = new Vector2(0, 450);
	private Vector2 entryVelocity;
	private Vector2 dampingForce = new Vector2(0, -1300);
	//private Vector2 smallForce = new Vector2(0, 0.000f);
	private AudioSource bigSplashSrc;
	private AudioSource smallSplashSrc;

	void Awake()
	{
		if (bigSplashNoise != null) {
			bigSplashSrc = gameObject.AddComponent<AudioSource>();
			bigSplashSrc.clip = bigSplashNoise;
			bigSplashSrc.volume = bigSplashVolume;
		}
		if (smallSplashNoise != null) {
			smallSplashSrc = gameObject.AddComponent<AudioSource>();
			smallSplashSrc.clip = smallSplashNoise;
			smallSplashSrc.volume = smallSplashVolume;
		}
	}

	void OnTriggerEnter2D(Collider2D c) {
		if (c.rigidbody2D == null)
			return;
		entryVelocity = c.gameObject.rigidbody2D.velocity;
		//Debug.Log (c.gameObject.rigidbody2D.velocity.y);
		//Handle noise
		if (Mathf.Abs(entryVelocity.y) > bigNoiseThreshold
			&& bigSplashSrc != null) {
			bigSplashSrc.Play();
		}
		else if (Mathf.Abs(entryVelocity.y) > noNoiseThreshold
				&& smallSplashSrc != null) {
			smallSplashSrc.Play();
		}

		if (Mathf.Abs(entryVelocity.y) < 4) {
			dampingForce.y = -1000;
			waterForce.y = 400;
			//c.gameObject.rigidbody2D.AddForce (waterForce);
		} else if (Mathf.Abs(entryVelocity.y) < 3 && entryVelocity.y < 0) {
			waterForce.y = -Physics2D.gravity.y;
			dampingForce.y = 0;
			//c.gameObject.rigidbody2D.AddForce (waterForce);
			//c.gameObject.rigidbody2D.velocity = new Vector2(c.gameObject.rigidbody2D.velocity.x, 0);
		} else {
			waterForce.y = 450;
			dampingForce.y = -1300;
			//c.gameObject.rigidbody2D.AddForce (waterForce);
		}
	}

	void OnTriggerStay2D(Collider2D c) {
		if (c.rigidbody2D == null)
			return;
		if (Mathf.Abs(entryVelocity.y) < 3 && entryVelocity.y < 0) {
			c.gameObject.rigidbody2D.AddForce (waterForce);
			//c.gameObject.rigidbody2D.velocity = new Vector2(c.gameObject.rigidbody2D.velocity.x, 0f);
			//this.collider2D.isTrigger = false;
		}
		else
			c.gameObject.rigidbody2D.AddForce (waterForce);
	}

	void OnTriggerExit2D(Collider2D c) {
		if (c.rigidbody2D == null)
			return;
		c.gameObject.rigidbody2D.AddForce(dampingForce);
	}
}
