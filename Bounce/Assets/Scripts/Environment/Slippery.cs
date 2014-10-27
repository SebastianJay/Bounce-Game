using UnityEngine;
using System.Collections;

public class Slippery : MonoBehaviour {
	private float moveForceSlippery = 50f;
	/*Amount of force to subtract from the total, is a subtraction instead of simply assigning
	  so that it could work with powerups that also influence the variable*/
	void Start () {
		PhysicsMaterial2D slippery = new PhysicsMaterial2D("Slippery");
		//Creates new material
		slippery.friction = 0f;
		//Sets its friction
		this.gameObject.collider2D.enabled = false;
		//Preps the collider
		this.gameObject.collider2D.sharedMaterial = slippery;
		//Sets the game object to have the new material
		this.gameObject.collider2D.enabled = true;
		//Resets the collider
		/*This material makes you slide, but does not influence human control (pressing a key)
		 * The only way to influence how fast a player can stop or turn on the surface when wanted is 
		 * by controlling the player ball control script, it seems, and then the move force*/
	}
	void OnCollisionEnter2D(Collision2D coll) {
		PlayerBallControl pbc = GameObject.FindWithTag ("Player").GetComponent<PlayerBallControl>();
		//Allows easy access of components
		pbc.moveForce -= moveForceSlippery;
		//Subtracts the move force from the player total
	}
	void OnCollisionExit2D(Collision2D coll) {
		PlayerBallControl pbc = GameObject.FindWithTag ("Player").GetComponent<PlayerBallControl>();
		//Allows easy access of components
		pbc.moveForce += moveForceSlippery;
		//Readds the amount that was subtracted to the move force, going back to normal
	}
}