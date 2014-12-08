using UnityEngine;
using System.Collections;

public class SelfRemove : MonoBehaviour {

	public float activeTime = 10f;
	private float activeTimer;
	
	// Update is called once per frame
	void Update () {
		activeTimer += Time.deltaTime;
		if (activeTimer > activeTime)
		{
			Destroy(gameObject);
		}
	}
}
