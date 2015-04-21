using UnityEngine;
using System.Collections.Generic;

public class Death : MonoBehaviour {

	//Remember to initialize this upon entering the level!
	public static Vector2 respawn;
	public static CameraFollowConfig camConfig;
	public static bool deathTransitioning = false;

	public float recoilForce = 12000f;
	public AudioClip deathNoise;
	public float deathVolume = 1.0f;
	private AudioSource deathSrc;

	private bool locked = false;
	private GameObject screenFadeObj;
	private GameObject camObj;
	private GameObject player;
	private GameObject escort;

	private Resettable[] resettables;

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
		resettables = FindObjectsOfType (typeof(Resettable)) as Resettable[];
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (CheckForDeath(collision.collider)) {
			collision.collider.gameObject.rigidbody2D.AddForce(collision.contacts[0].normal * recoilForce);
		}
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		CheckForDeath(col);
	}

	bool CheckForDeath(Collider2D col)
	{
		if ((col.tag == "Player" || col.tag == "MotherFollower") && !locked && 
		    (screenFadeObj == null || !screenFadeObj.GetComponent<ScreenFading>().IsTransitioning())) {

			if (deathSrc != null)
				deathSrc.Play();
			if (screenFadeObj != null)
			{
				locked = true;
				screenFadeObj.GetComponent<ScreenFading>().Transition(DeathTransition);
			}
			else
				DeathTransition();
			return true;
		}
		if (col.gameObject.layer == 10) {
			//the balloon layer -- hardcoded as 10
			player.GetComponent<PowerupManager>().EndPowerup();
			Destroy (col.gameObject);
			//probably could use popping noise here
		}
		return false;
	}

	void DeathTransition()
	{
		if (player == null)
			player = GameObject.FindGameObjectWithTag ("Player");
		if (player.GetComponent<PowerupManager>() != null)
			player.GetComponent<PowerupManager>().EndPowerup();
		if (player.GetComponent<PlayerBallControl>() != null) {
			player.GetComponent<PlayerBallControl>().ForceUndoDeformation ();
			if (player.GetComponent<PlayerBallControl>().npcTalker != null) {
				player.GetComponent<PlayerBallControl>().npcTalker.GetComponent<Interactable>().ForceQuitConvo();
			}
		}
		if (player.GetComponent<PlayerBodyControl>() != null) {
			if (player.GetComponent<PlayerBodyControl>().npcTalker != null) {
				player.GetComponent<PlayerBodyControl>().npcTalker.GetComponent<Interactable>().ForceQuitConvo();
			}
		}
		player.transform.position = respawn;
		player.rigidbody2D.velocity = Vector2.zero;
		player.rigidbody2D.angularVelocity = 0f;
		// do other possible resets!

		foreach (Resettable r in resettables) {
			r.Reset ();
		}

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
