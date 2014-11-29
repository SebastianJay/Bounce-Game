using UnityEngine;
using System.Collections.Generic;

public class Death : MonoBehaviour {

	//Remember to initialize this upon entering the level!
	public static Vector2 respawn;
	public static CameraFollowConfig camConfig;

	private bool locked = false;
	private GameObject screenFadeObj;
	private GameObject camObj;
	private GameObject player;
	private GameObject escort;

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
		if (col.tag == "Player" && !locked) {

			//player = col.transform;
			if (screenFadeObj != null)
			{
				locked = true;
				screenFadeObj.GetComponent<ScreenFading>().Transition(DeathTransition);
			}
			else
				DeathTransition();
		}
		else if (col.tag == "MotherFollower" && !locked)	//special case.. bla
		{
			//player = GameObject.FindGameObjectWithTag("Player").transform;
			//escort = col.transform;
			if (screenFadeObj != null)
			{
				locked = true;
				screenFadeObj.GetComponent<ScreenFading>().Transition(DeathWithEscortTransition);
			}
			else
				DeathWithEscortTransition();
		}
	}

	void DeathTransition()
	{
		player.transform.position = respawn;
		player.rigidbody2D.velocity = Vector2.zero;
		player.GetComponent<PowerupManager>().EndPowerup();
		// do other possible resets!

		camObj.GetComponent<CameraFollow>().loadConfig(camConfig);

		locked = false;
	}

	void DeathWithEscortTransition()
	{
		player.transform.position = respawn;
		player.rigidbody2D.velocity = Vector2.zero;
		player.GetComponent<PowerupManager>().EndPowerup();
		// do other possible resets!

		escort.transform.position = new Vector3(246f, 2.3f, 0f);	//we'll hard-code this one..
		escort.rigidbody2D.velocity = Vector2.zero;
		escort.GetComponent<FollowAI>().enabled = false;

		camObj.GetComponent<CameraFollow>().loadConfig(camConfig);

		locked = false;
	}
}