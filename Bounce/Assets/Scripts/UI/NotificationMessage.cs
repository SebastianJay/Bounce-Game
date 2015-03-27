using UnityEngine;
using System.Collections;

public class NotificationMessage : MonoBehaviour {

	public float showTime = 3f;
	public float disappearRate = 5f;
	public float alphaThreshold = 0.01f;
	private float timer = 0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if (timer > showTime) {
			GetComponent<GUIText>().color.a = Mathf.Lerp(GetComponent<GUIText>().color.a, 0f, Time.deltaTime * disappearRate);
			if (GetComponent<GUIText>().color.a < alphaThreshold) {
				Destroy(gameObject);
			}
		}
	}
}
