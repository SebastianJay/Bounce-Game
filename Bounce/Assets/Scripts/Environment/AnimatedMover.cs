using UnityEngine;
using System.Collections;

/// <summary>
/// Lets us specify movement of this GameObject to a destination over a period of time
/// Invoked programmatically via the MoveAbsolute or MoveRelative methods
/// </summary>
public class AnimatedMover : MonoBehaviour {

	private Vector3 start;
	private Vector3 stop;
	private float moveTime = 1.0f;
	public bool quadratic = false;	//if true, "accelerated movement"
									//otherwise linear, "constant speed"
	public AudioClip moveNoise;
	public float moveVolume = 1f;
	private float timer;
	private bool isMoving;
	private AudioSource moveSrc;

	void Awake() {
		if (moveNoise != null) {
			moveSrc = gameObject.AddComponent<AudioSource>();
			moveSrc.clip = moveNoise;
			moveSrc.volume = moveVolume;
		}
	}

	// Update is called once per frame
	void Update () {
		if (isMoving)
		{
			timer += Time.deltaTime;
			if (timer > moveTime)
			{
				transform.position = stop;
				isMoving = false;
			}
			else
			{
				float frac;
				if (quadratic)
					frac = Mathf.Clamp((timer * timer) / (moveTime * moveTime), 0f, 1f);
				else
					frac = Mathf.Clamp(timer / moveTime, 0f, 1f);
				transform.position = Vector3.Lerp(start, stop, frac);
			}
		}
	}

	public void MoveAbsolute(Vector3 dest, float time)
	{
		start = transform.position;
		stop = dest;
		moveTime = time;
		isMoving = true;
		timer = 0f;
		if (moveSrc != null)
			moveSrc.Play();
	}

	public void MoveRelative(Vector3 offset, float time)
	{
		start = transform.position;
		stop = transform.position + offset;
		moveTime = time;
		isMoving = true;
		timer = 0f;
		if (moveSrc != null)
			moveSrc.Play();
	}
}
