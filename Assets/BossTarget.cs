using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTarget : MonoBehaviour, IPlayerHittable {
	public void Update(){}
	public AbstractBoss boss;
	public void MeleeHit(int damage) {
		if (!enabled) {
			return;
		}
		boss.GetHit();
	}
	public void MissileHit(int damage) {}
}
