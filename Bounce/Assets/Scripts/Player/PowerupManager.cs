using UnityEngine;
using System.Collections;

public class PowerupManager : MonoBehaviour {

	public PowerupType currentPowerup = PowerupType.Normal;

	public float superJumpBoost = 3500f;
	public float superJumpTime = 10f;	//in seconds

	private int jumpBoost = 5;
	//Factor to increase jump height by

	private GameObject player;
	private float normalJumpForce;
	void Start()
	{
		player = GameObject.FindGameObjectWithTag ("Player");
		normalJumpForce = player.GetComponent<PlayerBallControl> ().jumpForce;
	}
	public void ActivatePowerup(PowerupType type)
	{
		currentPowerup = type;
		PlayerBallControl pbc = player.GetComponent<PlayerBallControl> ();
		switch (type)
		{
		case PowerupType.SuperJump:
			pbc.jumpForce = superJumpBoost;
			StartCoroutine(EndOfPowerup(superJumpTime, 0));
			break;
		}
	}

	void Update () 
	{
		/*
		PlayerBallControl pbc = GameObject.FindWithTag ("Player").GetComponent<PlayerBallControl>();
		//Gets access to the control script
		if (pbc.powerupState == PowerupType.SuperJump)
			//Accesses if there's a current power up type assigned
		{
			pbc.jumpForce += jumpBoost;
			//Adds the force factor to the control script
			StartCoroutine(EndOfPowerup(10.0f, 0));
			//Begins end of powerup coroutine, allows for waiting until powerup is finished, gives power up identifier, "0"
		}
		//If statement so that the script can be used with any powerup, add ifs as needed
		*/
	}

	IEnumerator EndOfPowerup(float waitTime, int powerType) {
		//Can be used with any powerup, accepts any time to end the powerup
		PlayerBallControl pbc = player.GetComponent<PlayerBallControl>();
		//Gets access to the control script
		yield return new WaitForSeconds(waitTime);
		//Script waits until time has passed, then ends the powerup
		if (powerType == 0) 
		{
			pbc.jumpForce = normalJumpForce;
			//Sets player to no superJump
		}
		//If statement so that the coroutine can be called with any powerup type, add more ifs as needed

		currentPowerup = PowerupType.Normal;
	}
}
