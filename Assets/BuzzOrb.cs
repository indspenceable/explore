using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuzzOrb : MonoBehaviour, IActivatableObject {
	public float TimeOff=4f;
	public float TimeOn=1f;
	public GameObject spike;
	public AudioClip buzz;
	public void Activate(Level l){
		StartCoroutine(ChangeSize());
	}

	private IEnumerator ChangeSize() {
		float dt;
		while (true){
			dt = 0;
			while (dt < TimeOff) {
				yield return null;
				dt += GameManager.instance.ActiveGameDeltaTime;
			}
			spike.SetActive(true);
			GameManager.instance.PlaySound(buzz);
			yield return null;
			dt = 0;
			while (dt < TimeOn) {
				yield return null;
				dt += GameManager.instance.ActiveGameDeltaTime;
			}
			spike.SetActive(false);
			yield return null;
		}
	}
}
