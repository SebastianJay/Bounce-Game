using UnityEngine;
using System.Collections;

public class PowerupPickUp : MonoBehaviour {
	
	public PowerupType type;
	
	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.tag == "Player")
		{
			Destroy (this.gameObject);
			PlayerBallControl pbc = col.gameObject.GetComponent<PlayerBallControl>();
			pbc.powerupState = type;
		}
	}
}
