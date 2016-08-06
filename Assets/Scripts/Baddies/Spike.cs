using UnityEngine;
using System.Collections;

public class Spike : MonoBehaviour {
	public void OnTriggerStay2D(Collider2D other) {
		if (other.gameObject == GameManager.instance.player.gameObject) {
			GameManager.instance.player.GetComponent<PlayerTakeDamage>().GetHit(1);
		}
		ICanHitPlayer onHit = GetComponent<ICanHitPlayer>();
		if (onHit != null) {
			onHit.ScoreHit();
		}
	}
}
