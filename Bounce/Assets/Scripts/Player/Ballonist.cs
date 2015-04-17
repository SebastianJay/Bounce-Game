using UnityEngine;
using System.Collections;

public class Ballonist : MonoBehaviour {

	public Transform balloonPrefab;
	public float moveForce = 300f;
	public float maxHorizDistance = 30f;
	public float maxVerticalDistance = 10f;

	private Transform balloonInst;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (balloonInst == null) {
			balloonInst = Instantiate(balloonPrefab, transform.position + new Vector3(0f, 2f, 0f), Quaternion.identity) as Transform;
		}
		float h = Input.GetAxis("Horizontal");
		if (h != 0) {
			balloonInst.rigidbody2D.AddForce(new Vector2(0f, moveForce * h));
		}
		if (Mathf.Abs(balloonInst.transform.position.x - transform.position.x) > maxHorizDistance) {
			transform.rigidbody2D.AddForce(new Vector2(0f, moveForce * Mathf.Sign(balloonInst.transform.position.x - transform.position.x)));
		}
		if (balloonInst.transform.position.y - transform.position.y > maxVerticalDistance) {
			transform.position = new Vector3(transform.position.x, balloonInst.transform.position.y - maxVerticalDistance, 0f);
		}
	}

	public void ForceQuit() {
		if (balloonInst != null) {
			Destroy (balloonInst.gameObject);
		}
	}
}
