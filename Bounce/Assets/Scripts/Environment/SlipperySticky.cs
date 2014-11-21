using UnityEngine;
using System.Collections;

public class SlipperySticky : MonoBehaviour {

	public bool slippery = true;	//if true, make slippery physics mat
									//if false, make sticky physics mat
	public float moveForce = 75f;
	public float stopMultiplier = 0.5f;
	public float frictionCoeff = 10.0f;

	private float restoreStopMultiplier = 0f;
	private float restoreForce = 0f;
	private float restoreFriction = 0f;

	/*Amount of force to subtract from the total, is a subtraction instead of simply assigning
	  so that it could work with powerups that also influence the variable*/
	void Start () {
		PhysicsMaterial2D frictionMat = new PhysicsMaterial2D("Slippery");
		//Creates new material
		if (this.slippery)
			frictionMat.friction = 0f;
		else
			frictionMat.friction = 1f;
		//Sets its friction
		this.gameObject.collider2D.enabled = false;
		//Preps the collider
		this.gameObject.collider2D.sharedMaterial = frictionMat;
		//Sets the game object to have the new material
		this.gameObject.collider2D.enabled = true;
		//Resets the collider

		PlayerBallControl pbc = GameObject.FindWithTag ("Player").GetComponent<PlayerBallControl>();
		if (pbc != null)
		{
			restoreStopMultiplier = pbc.translationStoppingMultiplier;
			restoreForce = pbc.moveForce;
			restoreFriction = pbc.frictionCoefficient;
		}
		/*This material makes you slide, but does not influence human control (pressing a key)
		 * The only way to influence how fast a player can stop or turn on the surface when wanted is 
		 * by controlling the player ball control script, it seems, and then the move force*/
	}
	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "Player")
		{
			PlayerBallControl pbc = coll.gameObject.GetComponent<PlayerBallControl>();
			if (pbc != null)
			{
				//adjust translation force values
				pbc.translationStoppingMultiplier = stopMultiplier;
				pbc.moveForce = moveForce;
				pbc.frictionCoefficient = frictionCoeff;
			}
		}
	}
	void OnCollisionExit2D(Collision2D coll) {
		if (coll.gameObject.tag == "Player")
		{
			PlayerBallControl pbc = coll.gameObject.GetComponent<PlayerBallControl>();
			if (pbc != null)
			{
				//restore pbc values
				pbc.translationStoppingMultiplier = restoreStopMultiplier;
				pbc.moveForce = restoreForce;
				pbc.frictionCoefficient = restoreFriction;
			}
		}
	}
}