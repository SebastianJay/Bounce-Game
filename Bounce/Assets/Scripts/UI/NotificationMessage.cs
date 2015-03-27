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
			float alpha = Mathf.Lerp(GetComponent<GUIText>().color.a, 0f, Time.deltaTime * disappearRate);
			GetComponent<GUIText>().color = new Color(GetComponent<GUIText>().color.r,
			                                          GetComponent<GUIText>().color.g,
			                                          GetComponent<GUIText>().color.b,
			                                          alpha);
			if (GetComponent<GUIText>().color.a < alphaThreshold) {
				Destroy(gameObject);
			}
		}
	}
}
