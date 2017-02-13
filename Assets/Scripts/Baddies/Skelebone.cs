using UnityEngine;
using System.Collections;

[RequireComponent(typeof(VerticalMovement))]
public class Skelebone : Shooter, IActivatableObject {
	public Transform rightGround;
	public Transform rightWall;
	public Transform leftGround;
	public Transform leftWall;
	public LayerMask walls;

	public float walkVelocity = 3f;
	public bool facingRight = true;

	public float gravity = 30f;
	private VerticalMovement vert;

	// Use this for initialization
	void Start () {
		vert = GetComponent<VerticalMovement>();
	}
	public void Activate(Level l) {
		GetComponent<SpriteRenderer>().flipX = !facingRight;
		StartCoroutine(Walk());
	}

	private bool checkCurrentDirection() {
		if (facingRight) {
			return Physics2D.OverlapPoint(rightGround.position, walls) && !Physics2D.OverlapPoint(rightWall.position, walls);
		} else {
			return Physics2D.OverlapPoint(leftGround.position, walls) && !Physics2D.OverlapPoint(leftWall.position, walls);
		}
	}

	public int scaleByDirection() {
		return facingRight ? 1 : -1;
	}

	private IEnumerator Fall() {
		while (!vert.CheckGrounded()) {
			vert.vy -= gravity * GameManager.instance.ActiveGameDeltaTime;
			vert.Fall(GameManager.instance.ActiveGameDeltaTime * vert.vy);
			yield return null;
		}
	}

	private IEnumerator Walk() {
		while (true) {
			float dt = 0f;
			float crawlPeriod = (float)Random.Range(1f, 3f);
			while (dt < crawlPeriod) {
				yield return null;
				dt += GameManager.instance.ActiveGameDeltaTime;
//				float pct = dt/crawlPeriod;
				transform.Translate(Vector3.right * walkVelocity * GameManager.instance.ActiveGameDeltaTime * scaleByDirection(), Space.World);
				if (!vert.CheckGrounded()) {
					// Start another coroutine
					yield return StartCoroutine(Fall());
					break;
				}

				if (!checkCurrentDirection()) {
					facingRight = !facingRight;
					GetComponent<SpriteRenderer>().flipX = !facingRight;
					this.speed *= -1;
//					break;
				}
			}
			yield return new WaitForSeconds(1f);
			if (Random.Range(1,2) == 1) {
				ShootFireball();
				yield return new WaitForSeconds(1f);
			}
			if (Random.Range(1,4) == 1) {
				facingRight = !facingRight;
				GetComponent<SpriteRenderer>().flipX = !facingRight;
			}
		}
	}
}
