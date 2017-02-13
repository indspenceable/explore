using UnityEngine;
using System.Collections;

public class Spike : MonoBehaviour {
	public int damage = 1;

	public void Update() {}
	public void OnTriggerStay2D(Collider2D other) {
		if (!enabled) {
			return;
		}
		if (other.gameObject == GameManager.instance.player.gameObject) {
			GameManager.instance.player.GetComponent<PlayerTakeDamage>().GetHit(damage);
		}
		ICanHitPlayer onHit = GetComponent<ICanHitPlayer>();
		if (onHit != null) {
			onHit.ScoreHit();
		}
	}
}
