using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class OneWay : MonoBehaviour {

	public const int DEFAULT_LAYER = 0;
	public const int PASS_THROUGH_LAYER = 13;

	private GameObject player;
	private Vector2 absScale;
	private Vector2 playerScale;
	private const float EPSILON = 1e-2f;
	private const float VELOCITY_THRESH = 0.0f;
	Vector2 GetAbsoluteScale(Transform original)
	{
		Vector2 retVal = Vector2.one;
		Transform trans = original;
		while (trans != null)
		{
			retVal.x *= trans.localScale.x;
			retVal.y *= trans.localScale.y;
			trans = trans.parent;
		}
		return retVal;
	}

	void Start()
	{
		player = GameObject.FindGameObjectWithTag ("Player");
		absScale = GetAbsoluteScale (this.transform);
		playerScale = GetAbsoluteScale (player.transform);
	}
	void Update () 
		//Has to check player velocity every update, else additional triggers would be necessary
	{
		if (player == null) {
			player = GameObject.FindGameObjectWithTag ("Player");
			playerScale = GetAbsoluteScale(player.transform);
		}
		//Debug.DrawLine(new Vector3(player.transform.position.x, player.transform.position.y - player.GetComponent<CircleCollider2D>().radius * playerScale.y, 0f),
		//              new Vector3(player.transform.position.x, transform.position.y + (GetComponent<BoxCollider2D>().center.y + GetComponent<BoxCollider2D>().size.y / 2) * absScale.y, 0f ), Color.cyan);
		//Debug.Log (player.rigidbody2D.velocity.y);
		//if the player is still or moving down, and his position is above this platform's position, then collide
		//Note that player must have a circle collider and platform must have box collider
		if (player.GetComponent<CircleCollider2D>() != null &&
			    /*player.rigidbody2D.velocity.y <= VELOCITY_THRESH && */
			    player.transform.position.y - player.GetComponent<CircleCollider2D>().radius * playerScale.y >= 
		    	transform.position.y + (GetComponent<BoxCollider2D>().center.y + GetComponent<BoxCollider2D>().size.y / 2) * absScale.y - EPSILON) {
				if (gameObject.layer != DEFAULT_LAYER) {
					//Debug.Log ("Player pos: "+(player.transform.position.y - player.GetComponent<CircleCollider2D>().radius));
					//Debug.Log ("Platform pos: "+(transform.position.y + GetComponent<BoxCollider2D>().center.y + GetComponent<BoxCollider2D>().size.y / 2 ));
				}
				this.gameObject.layer = DEFAULT_LAYER;
				//switches to the default (collidable) layer
		} 
		else if (player.GetComponent<BoxCollider2D>() != null && 
				/*player.rigidbody2D.velocity.y <= 0 &&*/ 
			    player.transform.position.y + (player.GetComponent<BoxCollider2D>().center.y - player.GetComponent<BoxCollider2D>().size.y / 2) * playerScale.y >= 
			    transform.position.y + (GetComponent<BoxCollider2D>().center.y + GetComponent<BoxCollider2D>().size.y / 2) * absScale.y - EPSILON) {
				this.gameObject.layer = DEFAULT_LAYER;
				//switches to the default (collidable) layer
		} 
		//otherwise, don't collide
		else {
			//if (gameObject.layer != PASS_THROUGH_LAYER) {
			//	Debug.Log ("Player pos: "+(player.transform.position.y - player.GetComponent<CircleCollider2D>().radius));
			//	Debug.Log ("Platform pos: "+(transform.position.y + GetComponent<BoxCollider2D>().center.y + GetComponent<BoxCollider2D>().size.y / 2) );
			//}
			this.gameObject.layer = PASS_THROUGH_LAYER;
			//switches to a special layer the player can pass through
		}
	}
}
