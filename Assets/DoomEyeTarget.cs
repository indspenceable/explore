using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoomEyeTarget : MonoBehaviour, IPlayerHittable {
	public void Update(){}
	public DoomEye main;
	public void MeleeHit(int damage) {
		if (!enabled) {
			return;
		}
		main.GetHit();
	}
	public void MissileHit(int damage) {}
}
