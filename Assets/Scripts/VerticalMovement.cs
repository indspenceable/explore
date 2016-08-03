using UnityEngine;
using System.Collections;

public class VerticalMovement : MonoBehaviour {
	public const float tinyMovementStep = 0.001f;

	public BoxCollider2D UpCollider;
	public BoxCollider2D DownCollider;
	public LayerMask levelGeometryMask;
	public LayerMask jumpThruPlatformMask;
	public float vy = 0f;

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
	public void Fall(float amt) {
		float i = 0f;
		Vector3 step = new Vector3(0f, -tinyMovementStep);
		while (i > amt && !CheckCollisionVerticalAtDistance(-tinyMovementStep)) {
			transform.Translate(step);
			i -= tinyMovementStep;
		}
	}

	public bool CheckCollisionVerticalAtDistance(float dv) {
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

	public void RiseOrFall(float amt)
	{
		if (amt > 0) {
			Rise (amt);
		} else if (amt < 0) {
			Fall (amt);
		}
	}

	void RestOnGround() {
		Vector3 step = new Vector3(0f, VerticalMovement.tinyMovementStep);
		while (CheckCollisionVerticalAtDistance(-tinyMovementStep))
			transform.Translate(step);
		transform.Translate(-step);
	}

	public bool CheckGrounded() {
		if (CheckCollisionVerticalAtDistance(-0.25f) && vy <= 0f) {
			Fall(-0.25f);
			RestOnGround();
			return true;
		}
		return false;
	}
}
