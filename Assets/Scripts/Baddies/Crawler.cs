using UnityEngine;
using System.Collections;

[RequireComponent(typeof(VerticalMovement))]
public class Crawler : MonoBehaviour, IActivatableObject {
	public Transform rightGround;
	public Transform rightWall;
	public Transform leftGround;
	public Transform leftWall;
	public LayerMask walls;

	public float crawlPeriod = 1f;
	public float crawlVelocityScale = 1f;
	public AnimationCurve crawlVelocity;
	public bool facingRight = true;

	public float gravity = 30f;
	private VerticalMovement vert;

	// Use this for initialization
	void Start () {
		vert = GetComponent<VerticalMovement>();
	}
	public void Activate() {
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
		float dt = 0f;
		while (dt < crawlPeriod) {
			yield return null;
			dt += GameManager.instance.ActiveGameDeltaTime;
			float pct = dt/crawlPeriod;
			transform.Translate(Vector3.right * crawlVelocity.Evaluate(pct) * GameManager.instance.ActiveGameDeltaTime * crawlVelocityScale * scaleByDirection(), Space.World);
			if (!vert.CheckGrounded()) {
				// Start another coroutine
				yield return StartCoroutine(Fall());
				break;
			}

			if (!checkCurrentDirection()) {
				facingRight = !facingRight;
				GetComponent<SpriteRenderer>().flipX = !facingRight;
				break;
			}
		}
		yield return StartCoroutine(Walk());
	}
}
