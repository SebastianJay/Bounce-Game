using UnityEngine;
using System.Collections;

/// <summary>
/// WORK IN PROGRESS.
/// Current implementation is probably not the way to go..
/// </summary>
public class Catapult : MonoBehaviour {

	public float dAngle=20f;
	public float moveTime=2f;
	public float relaxTime=5f;

	private float timer=0f;
	private bool timerStarted=false;
	private bool relaxing=false;

	private float startAngle;
	private float endAngle;
	// Use this for initialization
	void Start () {
		startAngle = transform.rotation.z;
		endAngle = transform.rotation.z + dAngle;
	}
	
	// Update is called once per frame
	void Update () {
		if (relaxing && timer < relaxTime) {
			timer += Time.deltaTime;
			transform.Rotate(0f, 0f, (startAngle - endAngle)/relaxTime * Time.deltaTime);
			//transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y,
			//	endAngle + (startAngle - endAngle) * Mathf.Clamp(timer/relaxTime, 0f, 1f), transform.rotation.w);
			if (timer >= relaxTime) {
				timerStarted = false;
				relaxing = false;
				timer = 0f;
			}
		}
		else if (timerStarted && timer < moveTime) {
			timer += Time.deltaTime;
			transform.Rotate(0f, 0f, (endAngle - startAngle)/moveTime * Time.deltaTime);
			//transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y,
			//	startAngle + (endAngle - startAngle) * Mathf.Clamp(timer/moveTime, 0f, 1f), transform.rotation.w);
			if (timer >= moveTime) {
				relaxing = true;
				timer = 0f;
			}
		}

	}


	void OnCollisionEnter2D(Collision2D coll) 
	{
		if (coll.gameObject.tag == "Player" && !relaxing) 
		{
			coll.transform.parent.parent = transform;
			this.timerStarted = true;
		}
		
	}
}
