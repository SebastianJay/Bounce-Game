using UnityEngine;
using System.Collections;

//Attach this component to the player
public class PowerupManager : MonoBehaviour {

	public PowerupType currentPowerup = PowerupType.Normal;

	public float superJumpBoost = 3500f;	//new jump force
	public float superJumpTime = 10f;	//in seconds

	private GameObject player;
	private float powerupTimer = 0f;
	private float powerupTime = 1f;
	private float normalJumpForce;
	void Start()
	{
		//player = GameObject.FindGameObjectWithTag ("Player");
		player = gameObject;
		normalJumpForce = this.GetComponent<PlayerBallControl> ().jumpForce;
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
			player.GetComponent<Spiderball>().activated = true;
			break;
		}
		powerupTimer = 0f;
	}

	void Update () 
	{
		if (currentPowerup != PowerupType.Normal)
		{
			powerupTimer += Time.deltaTime;
			if (powerupTimer >= powerupTime)
			{
				EndPowerup();
			}
		}
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
			this.GetComponent<Spiderball>().activated = false;
			break;
		}
		currentPowerup = PowerupType.Normal;
	}
}
