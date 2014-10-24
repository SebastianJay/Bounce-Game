using UnityEngine;
using System.Collections;

public class PowerupPickUp : MonoBehaviour {
	
	public PowerupType type;
	
	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.tag == "Player")
		{
			PowerupManager cpu = col.gameObject.GetComponent<PowerupManager>();
			//mod this later so player is enabled to activate powerup through a keystroke
			cpu.ActivatePowerup(type);
			//Destroy(this.gameObject);	//temporary!
			//Debug.Log ("Destroyed myself");
		}
	}

	void OnDestroy()
	{
		//Debug.Log ("Within OnDestroy()");
	}

	IEnumerator Resurrect(float waitTime)
	{
		return null;
	}
}
