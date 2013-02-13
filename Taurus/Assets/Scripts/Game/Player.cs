using UnityEngine;
using System.Collections;

public class Player : ActorMove {
    public bool opposite = false;

    public LayerMask blockCheck;

    private bool mDead = false;
    private bool mCrying = false;
    private bool mOnGoal = false;

    public bool dead { get { return mDead; } }
    public bool crying { get { return mCrying; } }

    public bool onGoal {
        get {
            return !(mDead || mCrying) && mOnGoal;
        }
    }

    public void Die(Dir fromDir) {
        if(!mDead) {
            Debug.Log("dead");
            StopMove();
            ProcessAct(Act.Die, fromDir, true);
            mDead = true;
        }
    }

    public void Cry() {
        if(!(mDead || mCrying)) {
            StopMove();
            ProcessAct(Act.Cry, Dir.South, true);
            mCrying = true;
        }
    }

    protected override void OnUndo(Act act, Dir dir) {
        switch(act) {
            case Act.Die:
                mDead = false;
                break;

            case Act.Cry:
                mCrying = false;
                break;

            case Act.Move:
                base.OnUndo(act, dir);
                break;
        }
    }

    protected override void OnInputAct(InputAction input, bool down) {
        moveActive = down;

        if(!mDead && !mCrying && down) {
            switch(input) {
                case InputAction.Up:
                    ProcessDir(Dir.North);
                    break;

                case InputAction.Down:
                    ProcessDir(Dir.South);
                    break;

                case InputAction.Left:
                    ProcessDir(opposite ? Dir.East : Dir.West);
                    break;

                case InputAction.Right:
                    ProcessDir(opposite ? Dir.West : Dir.East);
                    break;

                case InputAction.Fire:
                    if(tile != null) {
                        Vector2 checkPos = GetTilePos(curDir);

                        RaycastHit hit;
                        if(Physics.Raycast(new Vector3(checkPos.x, checkPos.y, -1000), Vector3.forward, out hit, Mathf.Infinity, blockCheck.value)) {
                            Block b = hit.transform.GetComponent<Block>();
                            b.Teleport();

                            ProcessAct(Act.Fire, curDir, false);
                        }
                    }
                    break;
            }
        }
    }

    protected override void OnMoveCellFinish() {
        mOnGoal = false;

        //check floor for danger
        tk2dRuntime.TileMap.TileInfo dat = tile.tileData;
        if(dat != null) {
            switch((TileType)dat.intVal) {
                case TileType.Spike:
                    PlayerController.KillPlayer(this, curDir);
                    break;

                case TileType.Goal:
                    mOnGoal = true;
                    break;
            }
        }
    }

    protected override void OnDestroy() {

        base.OnDestroy();
    }

    protected override void Start() {
        base.Start();

    }
}
