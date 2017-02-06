using UnityEngine;
using System.Collections;

public class Fireball : MonoBehaviour, IPlayerHittable {
	public Vector3 mv;
	bool amReversed = false;

	public void OnTriggerStay2D(Collider2D other) {
		if (other.gameObject == GameManager.instance.player.gameObject && !amReversed) {
			GameManager.instance.player.GetComponent<PlayerTakeDamage>().GetHit(1);
		}
//		ICanHitPlayer onHit = GetComponent<ICanHitPlayer>();
//		if (onHit != null && !amReversed) {
//			onHit.ScoreHit();
//		}
	}

	public IEnumerator DestroyIn(float dt) {
		yield return new WaitForSeconds(dt);
		Destroy(gameObject);
	}

	public void Update()  {
		transform.position += mv * Time.deltaTime;
	}

	public void MeleeHit(int damage) {
		if (!amReversed) {
			this.mv *= -1;
			amReversed = true;
			StopAllCoroutines();
			StartCoroutine(DestroyIn(1f));
		}
	}
	public void MissileHit(int damage){}
}
