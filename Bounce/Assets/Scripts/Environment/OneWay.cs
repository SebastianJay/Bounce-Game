using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class OneWay : MonoBehaviour {

	private GameObject player;
	void Start()
	{
		player = GameObject.FindGameObjectWithTag ("Player");
	}
	void Update () 
		//Has to check player velocity every update, else additional triggers would be necessary
	{
		//if the player is still or moving down, and his position is above this platform's position, then collide
		//Note that player must have a circle collider and platform must have box collider
		if (player.rigidbody2D.velocity.y <= 0 && 
		    player.transform.position.y - player.GetComponent<CircleCollider2D>().radius >= 
		    transform.position.y + GetComponent<BoxCollider2D>().size.y) {
			this.gameObject.layer = 0;
			//switches to the default (collidable) layer
		} 
		//otherwise, don't collide
		else {
			this.gameObject.layer = 13;
			//switches to a special layer the player can pass through
		}
	}
}
