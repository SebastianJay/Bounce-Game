using UnityEngine;
using System.Collections;

public class PowerupPickUp : MonoBehaviour {

	public PowerupType type;
	public bool respawns = true;
	public float respawnTime = 10.0f;
	public AudioClip pickUpNoise;
	public float pickUpVolume = 1f;

	private GameObject screenFadeObj;
	void Start()
	{
		screenFadeObj = GameObject.FindGameObjectWithTag ("ScreenFader");
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.tag == "Player"
		    && (screenFadeObj == null || !screenFadeObj.GetComponent<ScreenFading>().IsTransitioning()))
		{
			if (pickUpNoise != null) {
				//we create a dummy object for the noise since this one will get killed
				GameObject obj = new GameObject();
				obj.transform.position = this.transform.position;
				AudioSource src = obj.AddComponent<AudioSource>();
				src.clip = pickUpNoise;
				src.volume = pickUpVolume;
				obj.AddComponent<SelfRemove>();	//default 10 s
				src.Play();
			}
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
