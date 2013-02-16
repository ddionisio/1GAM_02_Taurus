﻿using UnityEngine;
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

    private List<Transform> mWaypoints;
    private int mCurWaypointInd;
    private TileAlign mCurWaypointTile;
    private Dir mWaypointDir;
    private bool mWaypointReverse = false;

    private bool mDead = false;

    public bool dead { get { return mDead; } }

    public void Die(Dir fromDir) {
        if(!mDead) {
            Debug.Log("enemy dead");
            StopMove();
            ProcessAct(Act.Die, fromDir, true);
            mDead = true;

            DetachPlayerMoveListen();
        }
    }

    protected override void OnUndo(Act act, Dir dir) {
        switch(act) {
            case Act.Die:
                mDead = false;
                AttachPlayerMoveListen();
                break;

            case Act.Move:
                base.OnUndo(act, dir);

                //determine if we need to update current waypoint...
                //check backwards from current
                bool wpReverse = mWaypointReverse;
                for(int i = 0, wpInd = mCurWaypointInd+(wpReverse ? 1 : -1); i < mWaypoints.Count; i++, wpInd = wpReverse ? wpInd+1 : wpInd-1) {
                    //check bounds
                    if(wpInd < 0) {
                        switch(waypointMode) {
                            case WaypointMode.Loop:
                                wpInd = mWaypoints.Count - 1;
                                break;

                            case WaypointMode.PingPong:
                                wpInd = 1;
                                wpReverse = !wpReverse;
                                break;
                        }
                    }
                    else if(wpInd >= mWaypoints.Count) {
                        switch(waypointMode) {
                            case WaypointMode.Loop:
                                wpInd = 0;
                                break;

                            case WaypointMode.PingPong:
                                wpInd = mWaypoints.Count - 1;
                                wpReverse = !wpReverse;
                                break;
                        }
                    }

                    Transform wp = mWaypoints[wpInd];
                    TileAlign ta = wp.GetComponent<TileAlign>();

                    int dCol = ta.col - tile.col;
                    int dRow = ta.row - tile.row;

                    if(dCol == 0 && dRow == 0) {
                        mCurWaypointInd = wpInd;

                        Transform node = mWaypoints[mCurWaypointInd];
                        mCurWaypointTile = node.GetComponent<TileAlign>();

                        SetWaypointDir();

                        break;
                    }
                }
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
                transform.position = mWaypoints[0].position;
            }
        }
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }

    protected override void OnMoveCellFinish() {
        foreach(Player p in PlayerController.players) {
            Dir d;
            if(p.state != State.Move && IsPlayerNeighbor(p, out d)) {
                DoKill(p, d);
                break;
            }
        }
    }

    protected override void OnDestroy() {
        DetachPlayerMoveListen();

        base.OnDestroy();
    }

    void OnPlayerMoveStart(ActorMove mover) {
        //move when player moves
        if(state != State.Move || state != State.PauseMove) {
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

                ProcessDir(moveTo);
            }
            else {
                if(mWaypoints != null && mCurWaypointTile == null)
                    InitWaypoint();

                if(CheckWaypointReached())
                    SetNextWaypointNode();

                DoMove();
            }
        }
    }

    void OnPlayerMoveFinish(ActorMove mover) {
        //check if we can eat the player
        /*Player p = mover as Player;
        Dir d;
        if(IsPlayerNeighbor(p, out d)) {
            DoKill(p, d);
        }*/
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
            else if(dCol == 0) { //up or down
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

        return doKill;
    }

    private void DoKill(Player p, Dir d) {
        PlayerController.KillPlayer(p, d);

        ProcessAct(Act.Kill, d, true);
    }


    private void AttachPlayerMoveListen() {
        if(PlayerController.players != null) {
            foreach(Player p in PlayerController.players) {
                p.moveStartCallback += OnPlayerMoveStart;
                p.moveFinishCallback += OnPlayerMoveFinish;
            }
        }
    }

    private void DetachPlayerMoveListen() {
        if(PlayerController.players != null) {
            foreach(Player p in PlayerController.players) {
                p.moveStartCallback -= OnPlayerMoveStart;
                p.moveFinishCallback -= OnPlayerMoveFinish;
            }
        }
    }

    private void SetWaypointDir() {
        if(mCurWaypointTile != null) {
            int dCol = mCurWaypointTile.col - tile.col;
            int dRow = mCurWaypointTile.row - tile.row;

            if(dCol < 0) {
                mWaypointDir = Dir.West;
            }
            else if(dCol > 0) {
                mWaypointDir = Dir.East;
            }
            else if(dRow < 0) {
                mWaypointDir = Dir.South;
            }
            else if(dRow > 0) {
                mWaypointDir = Dir.North;
            }
        }
    }

    private void InitWaypoint() {
        //first time
        
        mCurWaypointInd = 0;
        Transform node = mWaypoints[mCurWaypointInd];
        mCurWaypointTile = node.GetComponent<TileAlign>();

        tile.Align();

        SetWaypointDir();
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

    private void SetNextWaypointNode() {
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

        //make sure it's still valid
        if(nextInd >= 0 && nextInd < mWaypoints.Count) {
            mCurWaypointInd = nextInd;
            Transform node = mWaypoints[mCurWaypointInd];
            mCurWaypointTile = node.GetComponent<TileAlign>();
        }

        SetWaypointDir();
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
