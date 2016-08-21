using UnityEngine;
using System.Collections;

public class PlayerInputManager : MonoBehaviour {
	bool disableInput = false;
	public float GetAxis(string axisName) {
		if (! disableInput) {
			return Input.GetAxis(axisName);
		}
		return 0;
	}
	public bool GetButtonDown(string buttonName) {
		if (! disableInput) {
			return Input.GetButtonDown(buttonName);
		}
		return false;
	}
	public bool GetButtonUp(string buttonName) {
		if (! disableInput) {
			return Input.GetButtonUp(buttonName);
		}
		return false;
	}
	public bool GetButton(string buttonName) {
		if (! disableInput) {
			return Input.GetButton(buttonName);
		}
		return false;
	}
}
