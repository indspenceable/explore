using UnityEngine;
using System.Collections;

public class Bullet : GameplayPausable {
	public Vector3 direction;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	public override void UnpausedUpdate () {
		transform.position += direction*Time.deltaTime;
	}
}
