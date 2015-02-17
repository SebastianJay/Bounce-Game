using UnityEngine;
using System.Collections;

//Attach this component to the player
public class PowerupManager : MonoBehaviour {

	public PowerupType currentPowerup = PowerupType.Normal;

	public float superJumpBoost = 3500f;	//new jump force
	public float superJumpTime = 10f;	//in seconds
	public float spiderballTime = 100f;	
	//public float glidingScale = .02f;
	public float glidingTime = 15f;

	private GameObject player;
	public float powerupTimer = 0f;
	public float powerupTime = 1f;
	private float normalJumpForce;

	private GameObject timeObj;

	void Start()
	{
		//player = GameObject.FindGameObjectWithTag ("Player");
		player = gameObject;
		normalJumpForce = this.GetComponent<PlayerBallControl> ().jumpForce;
		timeObj = GameObject.FindGameObjectWithTag("PowerupTimer");
	}

	public void ActivatePowerup(PowerupType type)
	{
		if (currentPowerup != PowerupType.Normal)
		{
			EndPowerup();
		}

		currentPowerup = type;
		PlayerBallControl pbc = player.GetComponent<PlayerBallControl> ();
		switch (currentPowerup)
		{
		case PowerupType.SuperJump:
			pbc.jumpForce = superJumpBoost;
			powerupTime = superJumpTime;
			break;
		
		case PowerupType.Spiderball:
			this.GetComponent<Spiderball>().enabled = true;
			powerupTime = spiderballTime;
			break;

		case PowerupType.Gliding:
			this.GetComponent<Parachuter>().enabled = true;
			powerupTime = glidingTime;
			break;
		}
		powerupTimer = 0f;
	}

	void Update () 
	{
		if (currentPowerup != PowerupType.Normal)
		{
			powerupTimer += Time.deltaTime;
			if (timeObj != null)
				timeObj.GetComponent<GUIText>().text = 
					("Time: " + string.Format("{0:0.##}", powerupTime - powerupTimer));

			if (powerupTimer >= powerupTime)
			{
				EndPowerup();
			}
		}
		//Gliding effect
		/*
		if (currentPowerup == PowerupType.Gliding)
		{
			if (player.rigidbody2D.velocity.y < 0f)
				player.rigidbody2D.gravityScale = glidingScale;
			else if (player.rigidbody2D.velocity.y > 0f)
				player.rigidbody2D.gravityScale = 1f;

		}
		*/
	}

	public void EndPowerup()
	{
		PlayerBallControl pbc = player.GetComponent<PlayerBallControl>();
		switch(currentPowerup)
		{
		case PowerupType.SuperJump:
			pbc.jumpForce = normalJumpForce;
			break;
		case PowerupType.Spiderball:
			this.GetComponent<Spiderball>().ForceQuit();
			this.GetComponent<Spiderball>().enabled = false;
			//this.GetComponent<Spiderball>().activated = false;
			//pbc.spiderball = false;
			break;
		case PowerupType.Gliding:
			this.GetComponent<Parachuter>().ForceQuit();
			this.GetComponent<Parachuter>().enabled = false;
			//player.rigidbody2D.gravityScale = 1f;
			break;
		}
		currentPowerup = PowerupType.Normal;

		if (timeObj != null)
			timeObj.GetComponent<GUIText>().text = "";
	}
}
