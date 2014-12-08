using UnityEngine;
using System.Collections;

public class Spring : MonoBehaviour {
	public float springForce = 4000f;
	public AudioClip contactNoise;
	public float noiseVolume = 1f;

	private float orientation;
	private Vector2 direction;
	private AudioSource noiseSrc;

	void Awake() {
		orientation = (this.gameObject.transform.rotation.eulerAngles.z);
		direction = new Vector2(- Mathf.Sin(orientation * Mathf.PI / 180), Mathf.Cos(orientation * Mathf.PI / 180));
		if (contactNoise != null){
			noiseSrc = gameObject.AddComponent<AudioSource> ();
			noiseSrc.clip = contactNoise;
			noiseSrc.volume = noiseVolume;
		}
	}
	
	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "Player")
		{
			coll.gameObject.rigidbody2D.AddForce (springForce * direction);
			if (noiseSrc != null)
				noiseSrc.Play();
		}
	}
}