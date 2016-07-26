using UnityEngine;
using System.Collections;

public class Spike : MonoBehaviour {
	public void OnTriggerStay2D(Collider2D other) {
		other.SendMessage("GetHit", 1, SendMessageOptions.DontRequireReceiver);
	}
}
