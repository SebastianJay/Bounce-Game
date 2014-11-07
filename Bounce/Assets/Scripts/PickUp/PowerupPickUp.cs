using UnityEngine;
using System.Collections;

public class PowerupPickUp : MonoBehaviour {
	
	public PowerupType type;
	public bool respawns = true;
	public float respawnTime = 10.0f;

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.tag == "Player")
		{
			PowerupManager cpu = col.gameObject.GetComponent<PowerupManager>();
			cpu.ActivatePowerup(type);
			if (respawns && transform.parent != null)
			{
				Spawner spawner = transform.parent.GetComponent<Spawner>();
				if (spawner != null)
					spawner.SpawnAfterTime(respawnTime);
			}
			Destroy(gameObject);
		}
	}
}
