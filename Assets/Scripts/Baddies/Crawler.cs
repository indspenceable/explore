using UnityEngine;
using System.Collections;

public class Crawler : MonoBehaviour {
	public Transform rightGround;
	public Transform rightWall;
	public Transform leftGround;
	public Transform leftWall;
	public LayerMask walls;

	public float crawlPeriod = 1f;
	public float crawlVelocityScale = 1f;
	public AnimationCurve crawlVelocity;
	public bool facingRight = true;


	// Use this for initialization
	void Start () {
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

	private IEnumerator Walk() {
		float dt = 0f;
		while (dt < crawlPeriod) {
			yield return null;
			dt += Time.deltaTime;
			float pct = dt/crawlPeriod;
			transform.Translate(Vector3.right * crawlVelocity.Evaluate(pct) * Time.deltaTime * crawlVelocityScale * scaleByDirection(), Space.World);
			if (!checkCurrentDirection()) {
				facingRight = !facingRight;
				GetComponent<SpriteRenderer>().flipX = !facingRight;
				break;
			}
		}
		yield return StartCoroutine(Walk());
	}
}
