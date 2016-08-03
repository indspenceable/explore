using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	public Vector3 direction;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	public void Update () {
		transform.position += direction*Time.deltaTime;
	}
}
