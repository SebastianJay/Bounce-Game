using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	public GameObject obj;			//the object to clone
	public bool constantFrequency = true;
	public float frequency = 10f;	//if constantFrequency, how often the object is spawned
									//otherwise, the respawn time of a pick-up
	public bool spawnImmediately = true;
	public bool spawnInTrigger = false;

	private bool invalidated = false;	//if not constantFrequency, used to let other objects call spawner
	private float waitTime = 1f;
	private float timer = 0f;
	private bool inTrigger = false;

	void Start () {
		if (spawnImmediately)
		{
			GameObject clone = Instantiate(obj, transform.position, transform.rotation) as GameObject;
			if (clone.GetComponent<PowerupPickUp>() != null)
				clone.GetComponent<PowerupPickUp>().respawnTime = frequency;
			clone.transform.parent = transform;
		}
	}

	void Update () {
		if (constantFrequency)
		{
			if (!spawnInTrigger || inTrigger)
			{
				timer += Time.deltaTime;
				if (timer >= frequency)
				{
					DoSpawn();
					timer = 0f;
				}
			}
		}
		else if (invalidated)
		{
			if (!spawnInTrigger || inTrigger)
			{
				timer += Time.deltaTime;
				if (timer >= waitTime)
				{
					GameObject clone = Instantiate(obj, transform.position, transform.rotation) as GameObject;
					clone.transform.parent = transform;
					clone.GetComponent<PowerupPickUp>().respawnTime = frequency;
					invalidated = false;
					timer = 0f;
				}
			}
		}
	}

	public virtual void DoSpawn() {
		GameObject clone = Instantiate(obj, transform.position, transform.rotation) as GameObject;
		clone.transform.parent = transform;
	}

	public void SpawnAfterTime(float time)
	{
		waitTime = time;
		invalidated = true;
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.tag == "Player")
		{
			inTrigger = true;
		}
	}

	void OnTriggerExit2D(Collider2D col)
	{
		if (col.tag == "Player")
		{
			inTrigger = false;
		}
	}
}
