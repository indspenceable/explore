using UnityEngine;
using System.Collections;

public class DoorCollider : MonoBehaviour {
	public GameManager.Direction direction;

	public void OnTriggerEnter2D(Collider2D other) {
		GameManager gm = GameManager.instance;
		gm.StartCoroutine(gm.DoorCollision(this));
	}
}
