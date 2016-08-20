using UnityEngine;
using System.Collections;

public class GameStateFlagsComponent : MonoBehaviour {
	public GameStateFlags state;
}
[System.Serializable]
public struct GameStateFlags {
	public int mapX;
	public int mapY;
	public bool highJumpEnabled;
	public bool doubleJumpEnabled;
	public bool magnetEnabled;
	public bool meleeAttackEnabled;
	public bool rangedAttackEnabled;
}
