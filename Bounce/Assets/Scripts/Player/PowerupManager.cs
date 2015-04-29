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
	public float balloonTime = 12f;

	public Color superJumpColor;
	public Color spiderBallColor;
	public Color glidingColor;
	public Color balloonColor=Color.white;
	public Transform particlePrefab;
	private Transform particleObj;

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
		Color particleColor = Color.white;
		switch (currentPowerup)
		{
		case PowerupType.SuperJump:
			pbc.jumpForce = superJumpBoost;
			powerupTime = superJumpTime;
			particleColor = superJumpColor;
			break;
		
		case PowerupType.Spiderball:
			this.GetComponent<Spiderball>().enabled = true;
			powerupTime = spiderballTime;
			particleColor = spiderBallColor;
			break;

		case PowerupType.Gliding:
			this.GetComponent<Parachuter>().enabled = true;
			powerupTime = glidingTime;
			particleColor = glidingColor;
			break;

		case PowerupType.Balloon:
			this.GetComponent<Ballonist>().enabled = true;
			powerupTime = balloonTime;
			particleColor = balloonColor;
			break;
		}
		particleObj = Instantiate (particlePrefab, transform.position, Quaternion.identity) as Transform;
		particleObj.particleSystem.startColor = particleColor;
		powerupTimer = 0f;
		if (timeObj != null) {
			timeObj.GetComponent<PowerUpTime>().StartTimer(powerupTime, particleColor);
		}
	}

	void Update () 
	{
		if (currentPowerup != PowerupType.Normal)
		{
			powerupTimer += Time.deltaTime;
			if (timeObj != null) {
				timeObj.GetComponent<PowerUpTime>().UpdateTimer(powerupTimer);
			}

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
		case PowerupType.Balloon:
			this.GetComponent<Ballonist>().exitCode = 0;
			this.GetComponent<Ballonist>().ForceQuit();
			this.GetComponent<Ballonist>().enabled = false;
			break;
		}
		currentPowerup = PowerupType.Normal;

		if (timeObj != null) {
			timeObj.GetComponent<PowerUpTime>().EndTimer();
		}
		if (particleObj != null) {
			particleObj.particleSystem.emissionRate = 0f;
			particleObj.GetComponent<SelfRemove>().enabled = true;
		}
	}
}
