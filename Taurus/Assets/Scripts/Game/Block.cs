using UnityEngine;
using System.Collections;

public class Block : Actor {
    public LayerMask teleCheck;

    private Mirror mMirror;

    public void Teleport() {
        Dir teleDir = SetTelePosition();

        //check if we drop on top of something
        RaycastHit hit;
        Vector3 pos = transform.position;
        if(Physics.Raycast(new Vector3(pos.x, pos.y, -1000), Vector3.forward, out hit, Mathf.Infinity, teleCheck.value)) {
            if(hit.transform.CompareTag(Layers.tagPlayer)) {
                Player p = hit.transform.GetComponent<Player>();
                PlayerController.KillPlayer(p, Dir.NumDir);
            }
            else if(hit.transform.CompareTag(Layers.tagEnemy)) {
                Enemy e = hit.transform.GetComponent<Enemy>();
                e.Die(Dir.NumDir);
            }
        }

        ProcessAct(Act.Teleport, teleDir, null, true);
    }

    protected override void OnUndo(Act act, Dir dir, object dat) {
        switch(act) {
            case Act.Teleport:
                SetTelePosition();
                break;
        }
    }

    /// <summary>
    /// Called by ActionManager when input of act and dir occurs. Actor will act accordingly and call ActionManager.ActAdd if needed.
    /// </summary>
    protected override void OnInputAct(InputAction input, bool down) {
    }

    protected override void Awake() {
        base.Awake();

        GameObject mirrorGo = GameObject.FindGameObjectWithTag(Layers.tagMirror);
        mMirror = mirrorGo.GetComponent<Mirror>();
    }

    protected override void Start() {
        base.Start();
    }

    private Dir SetTelePosition() {
        int dCol = mMirror.tile.col - tile.col;
        Dir teleDir = dCol > 0 ? Dir.East : Dir.West;

        int newCol = mMirror.tile.col + dCol;

        tile.Set(newCol, tile.row);

        return teleDir;
    }
}
