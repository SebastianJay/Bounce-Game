using UnityEngine;
using System.Collections;

public class CollisionPickUp : MonoBehaviour {
	public int info1 = 1;
	public int info2 = 2;
	public char info3 = 'a';


	void OnTriggerEnter2D (Collider2D col)
	{
		Destroy (this.gameObject);
		col.gameObject.GetComponent<PlayerBallControl> ().infoOne = info1;
		Debug.Log ("Setting infoOne to 1");
		col.gameObject.GetComponent<PlayerBallControl> ().infoTwo = info2;
		Debug.Log ("Setting infoTwo to 2");
		col.gameObject.GetComponent<PlayerBallControl> ().infoThree = info3;
		Debug.Log ("Setting infoThree to a");
	
	}
}
