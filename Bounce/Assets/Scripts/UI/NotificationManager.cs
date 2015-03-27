using UnityEngine;
using System.Collections.Generic;

public class NotificationManager : MonoBehaviour {

	public Transform messagePrefab;
	public List<Transform> messages = new List<Transform>();

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

	public void PushMessage(string message)
	{
		Transform obj = Instantiate (messagePrefab, new Vector3 (0f, 0f, 0f), Quaternion.identity);

	}
}
