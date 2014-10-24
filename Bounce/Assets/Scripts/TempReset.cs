using UnityEngine;
using System.Collections;

public class TempReset : MonoBehaviour {

	private GameObject player;
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Reset"))
		{
			player.transform.position = this.transform.position;
			player.rigidbody2D.velocity = Vector2.zero;
		}
	}
}
