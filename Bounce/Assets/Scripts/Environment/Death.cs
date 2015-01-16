using UnityEngine;
using System.Collections.Generic;

public class Death : MonoBehaviour {

	//Remember to initialize this upon entering the level!
	public static Vector2 respawn;
	public static CameraFollowConfig camConfig;

	public AudioClip deathNoise;
	public float deathVolume = 1.0f;
	private AudioSource deathSrc;

	private bool locked = false;
	private GameObject screenFadeObj;
	private GameObject camObj;
	private GameObject player;
	private GameObject escort;

	void Awake()
	{
			if (deathNoise != null) {
				deathSrc = gameObject.AddComponent<AudioSource>();
				deathSrc.clip = deathNoise;
				deathSrc.volume = deathVolume;
			}
	}

	void Start()
	{
		screenFadeObj = GameObject.FindGameObjectWithTag ("ScreenFader");
		camObj = GameObject.FindGameObjectWithTag ("MainCamera");
		player = GameObject.FindGameObjectWithTag ("Player");
		escort = GameObject.FindGameObjectWithTag ("MotherFollower");

	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		CheckForDeath(collision.collider);
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		CheckForDeath(col);
	}

	void CheckForDeath(Collider2D col)
	{
		if ((col.tag == "Player" || col.tag == "MotherFollower") && !locked) {

			if (deathSrc != null)
				deathSrc.Play();
			if (screenFadeObj != null)
			{
				locked = true;
				screenFadeObj.GetComponent<ScreenFading>().Transition(DeathTransition);
			}
			else
				DeathTransition();
		}
	}

	void DeathTransition()
	{
		player.transform.position = respawn;
		player.rigidbody2D.velocity = Vector2.zero;
		player.rigidbody2D.angularVelocity = 0f;
		player.GetComponent<PowerupManager>().EndPowerup();
		// do other possible resets!

		if (escort != null && escort.GetComponent<FollowAI>().enabled)
		{
			escort.transform.position = new Vector3(246f, 2.3f, 0f);	//we'll hard-code this one..
			escort.rigidbody2D.velocity = Vector2.zero;
			escort.GetComponent<FollowAI>().enabled = false;
		}

		camObj.GetComponent<CameraFollow>().LoadConfig(camConfig);
		camObj.transform.position = new Vector3 (Mathf.Clamp (player.transform.position.x, camConfig.minXAndY.x, camConfig.maxXAndY.x),
		                                         Mathf.Clamp (player.transform.position.y, camConfig.minXAndY.y, camConfig.maxXAndY.y),
		                                         -10f);

		locked = false;
	}
}
