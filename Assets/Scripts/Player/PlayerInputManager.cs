using UnityEngine;
using System.Collections;

public class PlayerInputManager : MonoBehaviour {
	public float GetAxis(string axisName, GameMode gameMode) {
		if (GameManager.instance.currentGameMode == gameMode) {
			return Input.GetAxis(axisName);
		}
		return 0f;
	}

	public bool GetButtonDown(string buttonName, GameMode gameMode) {
		if (GameManager.instance.currentGameMode == gameMode) {
			return Input.GetButtonDown(buttonName);
		}
		return false;
	}

	public bool GetButtonUp(string buttonName, GameMode gameMode) {
		if (GameManager.instance.currentGameMode == gameMode) {
			return Input.GetButtonUp(buttonName);
		}
		return false;
	}

	public bool GetButton(string buttonName, GameMode gameMode) {
		if (GameManager.instance.currentGameMode == gameMode) {
			return Input.GetButton(buttonName);
		}
		return false;
	}
}
