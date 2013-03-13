using UnityEngine;
using System.Collections;

public class Player : ActorMove {
    public bool opposite = false;
    public bool oppositeVert = false;

    public LayerMask blockCheck;

    public GameObject onGoalObject;

    public GameObject onSecretObject;

    public GameObject onSecretMadeObject; //when both players touched secret

    public GameObject teleBlockHighlighterObject;

    private bool mDead = false;
    private bool mCrying = false;
    private bool mOnGoal = false;
    private bool mSecretTouched = false;

    public bool secretTouched { get { return mSecretTouched; } }
    public bool dead { get { return mDead; } }
    public bool crying { get { return mCrying; } }

    public bool onGoal {
        get {
            return !(mDead || mCrying) && mOnGoal;
        }
    }

    public void Die() {
        if(!mDead) {
            Debug.Log("dead");
            moveActive = false;
            StopMove();
            ProcessAct(Act.Die, curDir, null, true, true);
            mDead = true;

            if(teleBlockHighlighterObject != null)
                teleBlockHighlighterObject.SetActive(false);
        }
    }

    public void Cry() {
        if(!(mDead || mCrying)) {
            moveActive = false;
            StopMove();
            ProcessAct(Act.Cry, curDir, null, true, true);
            mCrying = true;

            if(teleBlockHighlighterObject != null)
                teleBlockHighlighterObject.SetActive(false);
        }
    }

    public void Victory() {
        ProcessAct(Act.Victory, Dir.South, null, false);

        if(teleBlockHighlighterObject != null)
            teleBlockHighlighterObject.SetActive(false);
    }

    protected override void OnUndo(Act act, Dir dir, object undoDat) {
        switch(act) {
            case Act.Die:
                mDead = false;
                break;

            case Act.Cry:
                mCrying = false;
                break;

            case Act.MoveDelayed:
                base.OnUndo(act, dir, undoDat);
                break;

            case Act.Move:
                base.OnUndo(act, dir, undoDat);

                TileCheck();

                if(onSecretMadeObject != null)
                    onSecretMadeObject.SetActive(false);

                HighlightBlockInFront();
                break;

            case Act.Fire:
                StartCoroutine(HighlightBlockInFrontDelay());
                break;
        }
    }

    protected override void OnInputAct(InputAction input, bool down) {
        moveActive = down;

        if(!mDead && !mCrying && down) {
            bool checkBlock = false;
            Transform bTrans;

            switch(input) {
                case InputAction.Up:
                    ProcessDir(oppositeVert ? Dir.South : Dir.North);
                    checkBlock = true;
                    break;

                case InputAction.Down:
                    ProcessDir(oppositeVert ? Dir.North : Dir.South);
                    checkBlock = true;
                    break;

                case InputAction.Left:
                    ProcessDir(opposite ? Dir.East : Dir.West);
                    checkBlock = true;
                    break;

                case InputAction.Right:
                    ProcessDir(opposite ? Dir.West : Dir.East);
                    checkBlock = true;
                    break;

                case InputAction.Fire:
                    bTrans = GetBlockInFront();
                    if(bTrans != null) {
                        Block b = bTrans.GetComponent<Block>();
                        if(b != null) {
                            StopMove();

                            b.Teleport();

                            ProcessAct(Act.Fire, curDir, null, true);    

                            if(teleBlockHighlighterObject != null)
                                teleBlockHighlighterObject.SetActive(false);
                        }
                    }
                    break;
            }

            //highlight block in front if we face a block
            if(checkBlock) {
                StartCoroutine(HighlightBlockInFrontDelay());
            }
        }
    }

    protected override void OnMoveCellFinish() {
        TileCheck();

        HighlightBlockInFront();
    }
        
    protected override void OnDestroy() {

        base.OnDestroy();
    }

    protected override void Start() {
        base.Start();

    }

    protected override void Awake() {
        base.Awake();

        if(onGoalObject != null)
            onGoalObject.SetActive(false);

        if(onSecretObject != null)
            onSecretObject.SetActive(false);

        if(onSecretMadeObject != null)
            onSecretMadeObject.SetActive(false);

        if(teleBlockHighlighterObject != null)
            teleBlockHighlighterObject.SetActive(false);
    }

    private IEnumerator HighlightBlockInFrontDelay() {
        yield return new WaitForFixedUpdate();

        HighlightBlockInFront();

        yield break;
    }

    public void HighlightBlockInFront() {
        if(teleBlockHighlighterObject != null) {
            Transform bTrans = GetBlockInFront();
            if(bTrans != null) {
                teleBlockHighlighterObject.SetActive(true);
                Vector3 pos = bTrans.transform.position;
                Vector3 hPos = teleBlockHighlighterObject.transform.position;
                teleBlockHighlighterObject.transform.position = new Vector3(pos.x, pos.y, hPos.z);
            }
            else {
                teleBlockHighlighterObject.SetActive(false);
            }
        }
    }

    private Transform GetBlockInFront() {
        Transform ret = null;

        if(tile != null) {
            Vector2 checkPos = GetTilePos(curDir);

            RaycastHit hit;
            if(Physics.Raycast(new Vector3(checkPos.x, checkPos.y, -1000), Vector3.forward, out hit, Mathf.Infinity, blockCheck.value)) {
                StopMove();

                ret = hit.transform;
            }
        }

        return ret;
    }

    private void TileCheck() {
        mOnGoal = false;
        mSecretTouched = false;

        //check floor for danger
        tk2dRuntime.TileMap.TileInfo dat = tile.tileData;
        if(dat != null) {
            switch((TileType)dat.intVal) {
                case TileType.Spike:
                    PlayerController.KillPlayer(this);
                    break;

                case TileType.Goal:
                    mOnGoal = true;
                    break;

                case TileType.Secret:
                    mSecretTouched = true;
                    break;
            }
        }

        if(onGoalObject != null)
            onGoalObject.SetActive(mOnGoal);

        if(onSecretObject != null)
            onSecretObject.SetActive(mSecretTouched);
    }
}
