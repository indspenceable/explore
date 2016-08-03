using UnityEngine;
using System.Collections;

public class PlatformPhysicsBase : MonoBehaviour {
	protected const float tinyMovementStep = 0.001f;

	public BoxCollider2D UpCollider;
	public BoxCollider2D DownCollider;
	public BoxCollider2D LeftCollider;
	public BoxCollider2D RightCollider;

	public LayerMask levelGeometryMask;
	public LayerMask jumpThruPlatformMask;
	public float vy = 0f;
	public float vx = 0f;

	protected void Rise(float amt) {
		float i = 0f;
		Vector3 step = new Vector3(0f, tinyMovementStep);
		while (i < amt && !CheckCollisionVerticalAtDistance(tinyMovementStep)) {
			transform.Translate(step);
			i += tinyMovementStep;
		}
		if (CheckCollisionVerticalAtDistance(tinyMovementStep) && vy > 0) {
			vy = 0;
		}
	}
	protected void Fall(float amt) {
		float i = 0f;
		Vector3 step = new Vector3(0f, -tinyMovementStep);
		while (i > amt && !CheckCollisionVerticalAtDistance(-tinyMovementStep)) {
			transform.Translate(step);
			i -= tinyMovementStep;
		}
	}

	protected bool CheckCollisionVerticalAtDistance(float dv) {
		// If we didn't set the jumpThruPlatformMask, then just use the standard LayerMask
		bool falling = (dv < 0) && (jumpThruPlatformMask != null);
		LayerMask mask = (falling ? jumpThruPlatformMask.value : 0) | levelGeometryMask.value;
		if (dv > 0) {
			return Physics2D.BoxCast((Vector2)UpCollider.transform.position + UpCollider.offset, 
				UpCollider.size, 0f, Vector2.up, dv, mask);
		} else {
			return Physics2D.BoxCast((Vector2)DownCollider.transform.position + DownCollider.offset,
				DownCollider.size, 0f, Vector2.up, dv, mask);
		}
	}

	protected void RiseOrFall(float amt)
	{
		if (amt > 0) {
			Rise (amt);
		} else if (amt < 0) {
			Fall (amt);
		}
	}

	protected void MoveLeftOrRight(float amt) {
		if (amt > 0) {
			moveRight(amt);
		} else if (amt < 0) {
			moveLeft(amt);
		}
	}

	protected bool CheckCollisionHorizontalAtDistance(float dv) {
		LayerMask mask = levelGeometryMask;

		if (dv > 0) {
			return Physics2D.BoxCast((Vector2)RightCollider.transform.position + RightCollider.offset, 
				LeftCollider.size, 0f, Vector2.right, dv, mask);
		} else {
			return Physics2D.BoxCast((Vector2)LeftCollider.transform.position + LeftCollider.offset,
				LeftCollider.size, 0f, Vector2.right, dv, mask);
		}
	}

	protected void moveRight(float amt) {
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
	void moveLeft(float amt) {
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
