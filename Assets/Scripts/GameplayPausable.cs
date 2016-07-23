using UnityEngine;
using System.Collections;

public abstract class GameplayPausable : MonoBehaviour {
	public static bool paused = false;
	// Update is called once per frame
	private bool wasPausedLastFrame = false;
	void Update () {
		if (wasPausedLastFrame != paused) {
			if (paused) {
				OnPause();
			} else {
				OnUnpause();
			}
			wasPausedLastFrame = paused;
		}
		if (paused) {
			PausedUpdate();
		} else {
			UnpausedUpdate();
		}
	}
	public virtual void OnPause() {}
	public virtual void OnUnpause() {}
	public virtual void PausedUpdate() {}
	public virtual void UnpausedUpdate() {}
}
