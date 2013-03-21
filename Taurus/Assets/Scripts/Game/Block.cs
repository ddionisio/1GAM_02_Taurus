using UnityEngine;
using System.Collections;

public class Block : Actor {
    public LayerMask teleCheck;
    public bool isVertical = false; //go up/down instead

    private Mirror mMirror;

    public void Teleport() {
        Dir teleDir = SetTelePosition();

        ProcessAct(Act.Teleport, teleDir, null, true);

        //check if we drop on top of something
        StartCoroutine(TeleRefresh());
    }

    private IEnumerator TeleRefresh() {
        yield return new WaitForFixedUpdate();

        RaycastHit hit;
        Vector3 pos = transform.position;
        if(Physics.Raycast(new Vector3(pos.x, pos.y, -1000), Vector3.forward, out hit, Mathf.Infinity, teleCheck.value)) {
            if(hit.transform.CompareTag(Layers.tagPlayer)) {
                Player p = hit.transform.GetComponent<Player>();
                PlayerController.KillPlayer(p);
            }
            else if(hit.transform.CompareTag(Layers.tagEnemy)) {
                Enemy e = hit.transform.GetComponent<Enemy>();
                e.Die();
            }
        }

        //update highlight for player nearby
        foreach(Player player in PlayerController.players)
            player.HighlightBlockInFront();

        yield break;
    }

    protected override void OnUndo(Act act, Dir dir, object dat) {
        switch(act) {
            case Act.Teleport:
                SetTelePosition();
                break;
        }
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
