using UnityEngine;
using System.Collections.Generic;

public class DynamicSpawner : Spawner {

	public List<Transform> objLst;
	public float rotVelMin = 10f;
	public float rotVelMax = 20f;
	public float transVelMin = 5f;
	public float transVelMax = 15f;
	public Vector2 moveDelta;

	public override void DoSpawn() {
		int index = Random.Range (0, objLst.Count);
		GameObject player = GameObject.FindGameObjectWithTag ("Player");
		float dx = transform.position.x - player.transform.position.x;
		float dy = transform.position.y - player.transform.position.y;
		float distance = Mathf.Sqrt (dx * dx + dy * dy);

		if(Mathf.Abs (distance) < 300f)
		{
			Transform clone = Instantiate (objLst [index], transform.position, Quaternion.identity) as Transform;
			if (clone.GetComponent<MovingPlatform>() != null)
			{
				float transVel = Random.Range(transVelMin, transVelMax);
				float transMoveTime = moveDelta.magnitude / transVel;
				clone.GetComponent<MovingPlatform>().useCurrentStartPosition = true;
				clone.GetComponent<MovingPlatform>().moveTime = transMoveTime;
				clone.GetComponent<MovingPlatform>().endPoint = moveDelta;
				if (clone.GetComponent<SelfRemove>() != null)
				{
					clone.GetComponent<SelfRemove>().activeTime = transMoveTime;
				}
				
			}
			if (clone.GetComponent<RotatingObject>() != null)
			{
				float rotVel = Random.Range(rotVelMin, rotVelMax);
				clone.GetComponent<RotatingObject>().rotationalVelocity = rotVel;
				clone.RotateAround(clone.transform.position, new Vector3(0f, 0f, 1f), Random.Range(0f, 360f));
			}
		}
	}
}
