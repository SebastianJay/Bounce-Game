using UnityEngine;
using System.Collections.Generic;

public class DynamicSpawner : Spawner {

	public List<Transform> objLst;
	public float rotVelMin = 10f;
	public float rotVelMax = 20f;
	public float transVelMin = 5f;
	public float transVelMax = 15f;
	public Vector2 moveDelta;

	public bool useObjectPool = true;
	public int poolSize = 15;
	private List<Transform> objectPool;
	private List<float> activeTimes;

	public void Start() {
		if (useObjectPool)
		{
			objectPool = new List<Transform> ();
			activeTimes = new List<float>();
			for (int i = 0; i < poolSize; i++)
			{
				int index = Random.Range (0, objLst.Count);
				Transform clone = Instantiate (objLst [index], transform.position, Quaternion.identity) as Transform;
				clone.gameObject.SetActive(false);
				objectPool.Add(clone);
				activeTimes.Add(-1.0f);
			}
		}
	}

	public void FixedUpdate()
	{
		if (useObjectPool)
		{
			for (int i = 0; i < activeTimes.Count; i++)
			{
				if (activeTimes[i] > 0) {
					activeTimes[i] -= Time.deltaTime;
					if (activeTimes[i] <= 0)
						objectPool[i].gameObject.SetActive(false);
				}
			}
		}
	}

	public override void DoSpawn() {
		Transform clone;
		int index;
		if (useObjectPool) {
			index = 0;
			while (index < activeTimes.Count && activeTimes[index] > 0)
				index++;
			//Debug.Log (index);
			if (index >= activeTimes.Count)
				return;
			clone = objectPool[index];
			clone.gameObject.SetActive(true);
		}
		else {
			index = Random.Range (0, objLst.Count);
			clone = Instantiate (objLst [index], transform.position, Quaternion.identity) as Transform;
		}

		if (clone.GetComponent<MovingPlatform>() != null)
		{
			float transVel = Random.Range(transVelMin, transVelMax);
			float transMoveTime = moveDelta.magnitude / transVel;
			if (useObjectPool)
			{
				activeTimes[index] = transMoveTime;
				clone.position = transform.position;
			}
			else if (clone.GetComponent<SelfRemove>() != null)
			{
				clone.GetComponent<SelfRemove>().activeTime = transMoveTime;
			}
			clone.GetComponent<MovingPlatform>().useCurrentStartPosition = true;
			clone.GetComponent<MovingPlatform>().moveTime = transMoveTime;
			clone.GetComponent<MovingPlatform>().endPoint = moveDelta;
			clone.GetComponent<MovingPlatform>().Reset();
		}
		if (clone.GetComponent<RotatingObject>() != null)
		{
			float rotVel = Random.Range(rotVelMin, rotVelMax);
			clone.GetComponent<RotatingObject>().rotationalVelocity = rotVel;
			clone.RotateAround(clone.transform.position, new Vector3(0f, 0f, 1f), Random.Range(0f, 360f));
		}
	}
}
