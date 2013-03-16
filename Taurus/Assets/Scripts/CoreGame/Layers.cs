using UnityEngine;
using System.Collections;

//put this with Main in a game object to persist between scenes
public class Layers : MonoBehaviour {
    public const string tagPlayer = "Player";
    public const string tagMirror = "mirror";
    public const string tagMap = "map";
    public const string tagEnemy = "enemy";
    public const string tagController = "GameController";

    //only use these after awake
    public static int ignoreRaycast;
    public static int player;
    public static int block;
    public static int enemy;


    void Awake() {
        ignoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
        player = LayerMask.NameToLayer("Player");
        block = LayerMask.NameToLayer("Block");
        enemy = LayerMask.NameToLayer("Enemy");
    }
}
