using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : ActorMove {
    public enum WaypointMode {
        Loop,
        PingPong
    }

    public string waypoint;
    public WaypointMode waypointMode;

    public Player copyPlayerMove;
    public bool copyReverseVertical;
    public bool copyReverseHorizontal;

    public bool dieOnKillFloor;

    public bool fireImmune = false;

    public bool harmless = false;

    private class WaypointDat {
        public int ind;
        public bool isReverse=false;
    }

    private List<Transform> mWaypoints;
    private int mCurWaypointInd;
    private TileAlign mCurWaypointTile;
    private Dir mWaypointDir;
    private bool mWaypointReverse = false;

    private bool mDead = false;

    public bool dead { get { return mDead; } }

    public void Die() {
        if(!mDead) {
            Debug.Log("enemy dead");
            StopMove();
            ProcessAct(Act.Die, curDir, mMoveDat, true, true);
            mDead = true;

            DetachPlayerMoveListen();
        }
    }

    protected override void OnUndo(Act act, Dir dir, object dat) {
        switch(act) {
            case Act.Die:
                mDead = false;
                AttachPlayerMoveListen();
                break;

            case Act.Kill:
                if(dir == Dir.NumDir)
                    ProcessAct(Act.Face, curDir, mMoveDat, false);
                break;

            case Act.MoveDelayed:
            case Act.Move:
                StopMove();

                base.OnUndo(act, dir, dat);

                int wpInd;
                if(dat != null) {
                    WaypointDat wdat = (WaypointDat)dat;
                    wpInd = wdat.ind;
                    mWaypointReverse = wdat.isReverse;
                    mMoveDat = dat;
                }
                else {
                    wpInd = 0;
                    mWaypointReverse = false;
                }

                if(mCurWaypointInd != wpInd) {
                    mCurWaypointInd = wpInd;

                    Transform node = mWaypoints[mCurWaypointInd];
                    mCurWaypointTile = node.GetComponent<TileAlign>();
                }

                SetWaypointDir();
                break;
        }
    }

    protected override void OnInputAct(InputAction input, bool down) {
    }

    // Use this for initialization
    protected override void Start() {
        base.Start();

        AttachPlayerMoveListen();

        if(!string.IsNullOrEmpty(waypoint)) {
            mWaypoints = WaypointManager.instance.GetWaypoints(waypoint);

            //put the enemy at the first node
            if(mWaypoints != null) {
                Vector3 pos = mWaypoints[0].position;
                transform.position = new Vector3(pos.x, pos.y, transform.position.z);

                StartCoroutine(InitWaypointDelayed());
            }
        }
    }
	
	IEnumerator InitWaypointDelayed() {
		yield return new WaitForFixedUpdate();
		
		InitWaypoint();
		
		yield break;
	}

    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }

    protected override void OnMoveCellFinish() {
        if(mWaypoints != null)
            RefreshFaceToWaypoint();

        bool processPlayerCheck = true;

        //check floor for danger
        if(dieOnKillFloor) {
            tk2dRuntime.TileMap.TileInfo dat = tile.tileData;
            if(dat != null) {
                switch((TileType)dat.intVal) {
                    case TileType.Spike:
                        Die();
                        processPlayerCheck = false;
                        break;
                }
            }
        }

        if(processPlayerCheck) {
            foreach(Player p in PlayerController.players) {
                Dir d;
                if(p.state != State.Move && IsPlayerNeighbor(p, out d)) {
                    DoKill(p, d);
                    break;
                }
            }
        }
    }

    protected override void OnDestroy() {
        DetachPlayerMoveListen();

        base.OnDestroy();
    }

    void OnPlayerMoveStart(ActorMove mover) {
        //move when player moves
        if(!mDead && state != State.Move && state != State.PauseMove) {
            if(mover == copyPlayerMove) {
                Dir moveTo = mover.curDir;

                if(copyReverseVertical) {
                    switch(moveTo) {
                        case Dir.North:
                            moveTo = Dir.South;
                            break;
                        case Dir.South:
                            moveTo = Dir.North;
                            break;
                    }
                }

                if(copyReverseHorizontal) {
                    switch(moveTo) {
                        case Dir.West:
                            moveTo = Dir.East;
                            break;
                        case Dir.East:
                            moveTo = Dir.West;
                            break;
                    }
                }

                ProcessDir(moveTo);
            }
            else if(mWaypoints != null) {
                if(CheckWaypointReached()) {
                    SetNextWaypointNode();

                    WaypointDat newWPDat = new WaypointDat();
                    newWPDat.ind = mCurWaypointInd;
                    newWPDat.isReverse = mWaypointReverse;
                    mMoveDat = (object)newWPDat;
                }

                DoMove();
            }
        }
    }

    void OnPlayerMoveFinish(ActorMove mover) {
        //check if we can eat the player
        if(!mDead && state != State.Move && state != State.PauseMove) {
            Player p = mover as Player;
            Dir d;
            if(IsPlayerNeighbor(p, out d)) {
                DoKill(p, d);
            }
        }
    }

    void OnPlayerCopyAct(Act act, Dir dir) {
        if(act == Act.Face) {
            Dir faceTo = dir;

            if(copyReverseVertical) {
                switch(faceTo) {
                    case Dir.North:
                        faceTo = Dir.South;
                        break;
                    case Dir.South:
                        faceTo = Dir.North;
                        break;
                }
            }

            if(copyReverseHorizontal) {
                switch(faceTo) {
                    case Dir.West:
                        faceTo = Dir.East;
                        break;
                    case Dir.East:
                        faceTo = Dir.West;
                        break;
                }
            }

            ProcessAct(Act.Face, faceTo, mMoveDat, false);
        }
    }

    private bool IsPlayerNeighbor(Player p, out Dir d) {
        d = Dir.NumDir;
        bool doKill = false;

        if(p != null) {
            int dCol = p.tile.col - tile.col;
            int dRow = p.tile.row - tile.row;

            if(dCol == 0 && dRow == 0) { //on top?
                doKill = true;
            }
            else if(!harmless) {
                if(dCol == 0) { //up or down
                    if(dRow == -1) {
                        d = Dir.South;
                        doKill = true;
                    }
                    else if(dRow == 1) {
                        d = Dir.North;
                        doKill = true;
                    }
                }
                else if(dRow == 0) { //left or right
                    if(dCol == -1) {
                        d = Dir.West;
                        doKill = true;
                    }
                    else if(dCol == 1) {
                        d = Dir.East;
                        doKill = true;
                    }
                }
            }
        }

        return doKill;
    }

    private void DoKill(Player p, Dir d) {
        PlayerController.KillPlayer(p);

        ProcessAct(Act.Kill, d, mMoveDat, true);
    }


    private void AttachPlayerMoveListen() {
        if(PlayerController.players != null) {
            foreach(Player p in PlayerController.players) {
                p.moveStartCallback += OnPlayerMoveStart;
                p.moveFinishCallback += OnPlayerMoveFinish;
            }
        }

        if(copyPlayerMove != null)
            copyPlayerMove.actCallback += OnPlayerCopyAct;
    }

    private void DetachPlayerMoveListen() {
        if(PlayerController.players != null) {
            foreach(Player p in PlayerController.players) {
                if(p != null) {
                    p.moveStartCallback -= OnPlayerMoveStart;
                    p.moveFinishCallback -= OnPlayerMoveFinish;
                }
            }
        }

        if(copyPlayerMove != null)
            copyPlayerMove.actCallback -= OnPlayerCopyAct;
    }

    private Dir GetDirTo(TileAlign toTile) {
        Dir ret = Dir.South;

        int dCol = toTile.col - tile.col;
        int dRow = toTile.row - tile.row;

        if(dCol < 0) {
            ret = Dir.West;
        }
        else if(dCol > 0) {
            ret = Dir.East;
        }
        else if(dRow > 0) {
            ret = Dir.North;
        }

        return ret;
    }

    private void SetWaypointDir() {
        if(mCurWaypointTile != null) {
            mWaypointDir = GetDirTo(mCurWaypointTile);

            ProcessAct(Act.Face, mWaypointDir, mMoveDat, false);
        }
    }

    private void InitWaypoint() {
        //first time
        
        mCurWaypointInd = 0;
        Transform node = mWaypoints[mCurWaypointInd];
        mCurWaypointTile = node.GetComponent<TileAlign>();

        tile.Align();

        RefreshFaceToWaypoint();
		ProcessAct(Act.Face, curDir, mMoveDat, false);
    }

    private bool CheckWaypointReached() {
        bool ret = false;

        if(mCurWaypointTile != null) {
            int dCol = mCurWaypointTile.col - tile.col;
            int dRow = mCurWaypointTile.row - tile.row;

            ret = dCol == 0 && dRow == 0;
        }

        return ret;
    }

    private int GetNextWaypointInd() {
        //determine next node depending on waypoint mode
        int nextInd = mWaypointReverse ? mCurWaypointInd - 1 : mCurWaypointInd + 1;
        if(nextInd == mWaypoints.Count) {
            switch(waypointMode) {
                case WaypointMode.Loop:
                    nextInd = 0;
                    break;
                case WaypointMode.PingPong:
                    nextInd = mCurWaypointInd - 1;
                    mWaypointReverse = !mWaypointReverse;
                    break;
            }
        }
        else if(nextInd < 0) {
            switch(waypointMode) {
                case WaypointMode.Loop:
                    nextInd = mWaypoints.Count - 1;
                    break;
                case WaypointMode.PingPong:
                    nextInd = 1;
                    mWaypointReverse = !mWaypointReverse;
                    break;
            }
        }

        return nextInd;
    }

    private void SetNextWaypointNode() {
        //determine next node depending on waypoint mode
        int nextInd = GetNextWaypointInd();

        //make sure it's still valid
        if(nextInd >= 0 && nextInd < mWaypoints.Count) {
            mCurWaypointInd = nextInd;
            Transform node = mWaypoints[mCurWaypointInd];
            mCurWaypointTile = node.GetComponent<TileAlign>();
        }

        SetWaypointDir();
    }

    private void RefreshFaceToWaypoint() {
        Dir d = mWaypointDir;

        if(CheckWaypointReached()) {
            int nextInd = GetNextWaypointInd();
            if(nextInd >= 0 && nextInd < mWaypoints.Count) {
                Transform node = mWaypoints[nextInd];
                TileAlign nextWpTile = node.GetComponent<TileAlign>();
                d = GetDirTo(nextWpTile);
            }
        }

        SetCurDir(d);
    }

    private void DoMove() {
        if(mWaypoints != null) {
            ProcessDir(mWaypointDir);
        }
        else {
            //random
            ProcessDir((Dir)Random.Range(0, (int)Dir.NumDir));
        }
    }
}
