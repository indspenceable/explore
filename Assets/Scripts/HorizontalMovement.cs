using UnityEngine;
using System.Collections;

public class HorizontalMovement : MonoBehaviour {
	public const float tinyMovementStep = 0.001f;

	public BoxCollider2D LeftCollider;
	public BoxCollider2D RightCollider;
	public float vx = 0f;
	public LayerMask levelGeometryMask;

	public void MoveLeftOrRight(float amt) {
		if (amt > 0) {
			moveRight(amt);
		} else if (amt < 0) {
			moveLeft(amt);
		}
	}

	public bool CheckCollisionHorizontalAtDistance(float dv) {
		LayerMask mask = levelGeometryMask;

		if (dv > 0) {
			return Physics2D.BoxCast((Vector2)RightCollider.transform.position + RightCollider.offset, 
				LeftCollider.size, 0f, Vector2.right, dv, mask);
		} else {
			return Physics2D.BoxCast((Vector2)LeftCollider.transform.position + LeftCollider.offset,
				LeftCollider.size, 0f, Vector2.right, dv, mask);
		}
	}

	public void moveRight(float amt) {
		float i = 0f;
		Vector3 step = new Vector3(tinyMovementStep, 0f);
		while (i < amt && !CheckCollisionHorizontalAtDistance(tinyMovementStep)) {
			transform.Translate(step);
			i += tinyMovementStep;
		}
		if (CheckCollisionHorizontalAtDistance(tinyMovementStep) && vx > 0) {
			vx = 0;
		}
	}
	public void moveLeft(float amt) {
		float i = 0f;
		Vector3 step = new Vector3(-tinyMovementStep, 0f);
		while (i > amt && !CheckCollisionHorizontalAtDistance(-tinyMovementStep)) {
			transform.Translate(step);
			i -= tinyMovementStep;
		}
		if (CheckCollisionHorizontalAtDistance(-tinyMovementStep) && vx < 0) {
			vx = 0;
		}
	}
}
