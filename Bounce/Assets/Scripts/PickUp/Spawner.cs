using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	public GameObject obj;			//the object to clone
	public bool constantFrequency = true;	
	public float frequency = 10f;	//if constantFrequency, how often the object is spawned
	public bool spawnImmediately = true;

	private bool invalidated = false;	//if not constantFrequency, used to let other objects call spawner
	private float waitTime = 1f;
	private float timer = 0f;

	void Start () {
		if (spawnImmediately)
		{
			GameObject clone = Instantiate(obj, transform.position, transform.rotation) as GameObject;
			clone.transform.parent = transform;
		}
	}
	
	void Update () {
		if (constantFrequency)
		{
			timer += Time.deltaTime;
			if (timer >= frequency)
			{
				GameObject clone = Instantiate(obj, transform.position, transform.rotation) as GameObject;
				clone.transform.parent = transform;
				timer = 0f;
			}
		}		
		else if (invalidated)
		{
			timer += Time.deltaTime;
			if (timer >= waitTime)
			{
				GameObject clone = Instantiate(obj, transform.position, transform.rotation) as GameObject;
				clone.transform.parent = transform;
				invalidated = false;
				timer = 0f;
			}
		}
	}

	public void SpawnAfterTime(float time)
	{
		waitTime = time;
		invalidated = true;
	}
}
