using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamePillar : MonoBehaviour, IActivatableObject {
	public float period = 3f;
	public float initialDelay = 0f;
	public float warmUp;

	public Spike spike;
	public SpriteRenderer pillarSprite;
	public ParticleSystem pillarParticleSystem;

	public void Activate(Level l) {
		StartCoroutine(SpitFire());
	}

	public IEnumerator SpitFire() {
		var em = pillarParticleSystem.emission;
//		em.enabled = true;
		UnityEngine.ParticleSystem.MinMaxCurve rateOverTime = em.rateOverTime;
		// Set everything off to start.
		spike.enabled = false;
		pillarSprite.enabled = false;
		em.rateOverTime = 0f;


		float dt = 0f;
		while (dt < initialDelay) {
			yield return null;
			dt += GameManager.instance.ActiveGameDeltaTime;
		}
		while (true) {
			em.rateOverTime = rateOverTime;
			dt = 0f;
			while (dt < warmUp) {
				yield return null;
				dt += GameManager.instance.ActiveGameDeltaTime;
			}
			spike.enabled = true;
			pillarSprite.enabled = true;
			dt = 0f;
			while (dt < period/2f - warmUp) {
				yield return null;
				dt += GameManager.instance.ActiveGameDeltaTime;
			}
			spike.enabled = false;
			pillarSprite.enabled = false;
			em.rateOverTime = 0f;
			dt = 0f;
			while (dt < period/2f) {
				yield return null;
				dt += GameManager.instance.ActiveGameDeltaTime;
			}
		}
	}
}
