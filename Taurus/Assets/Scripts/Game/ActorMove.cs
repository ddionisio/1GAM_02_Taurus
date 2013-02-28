﻿using UnityEngine;
using System.Collections;

public abstract class ActorMove : Actor {
    public enum State {
        None,
        PauseMove,
        Move,
        Face
    }

    public delegate void MoveCallback(ActorMove mover);

    public const int slowMaxCount = 1;

    public bool ignoreWalls = false;

    public float moveDelay = 0.3f;
    public float pauseMoveDelay = 0.15f;
    public float faceDelay = 1.0f;

    public LayerMask solidCheck;

    public event MoveCallback moveStartCallback;
    public event MoveCallback moveFinishCallback;

    protected object mMoveDat = null;

    private bool mMoveActive;
    private State mCurState = State.None;
    private Dir mCurDir = Dir.South;
    private float mCurMoveTime = 0.0f;
    private Vector2 mMoveStartPos;
    private Vector2 mMoveEndPos;
    private int mNextCol;
    private int mNextRow;
    private int mSlowCounter = 0;
            
    public State state { get { return mCurState; } }
    public Dir curDir { get { return mCurDir; } }
    public bool moveActive { get { return mMoveActive; } set { mMoveActive = value; } }

    public void StopMove() {
        if(mCurState == State.Move) {
            transform.position = new Vector3(mMoveEndPos.x, mMoveEndPos.y, transform.position.z);
            if(tile != null)
                tile.Set(mNextCol, mNextRow);
        }

        mCurState = State.None;
    }

    public Vector3 GetTilePos(Dir dir) {
        Vector3 newPos = transform.position;

        tk2dTileMap map = TileInfo.instance.map;

        float amtX = map.partitionSizeX;
        float amtY = map.partitionSizeY;

        switch(dir) {
            case Dir.North:
                newPos.y += amtY;
                break;

            case Dir.South:
                newPos.y -= amtY;
                break;

            case Dir.East:
                newPos.x += amtX;
                break;

            case Dir.West:
                newPos.x -= amtX;
                break;
        }

        return newPos;
    }

    /// <summary>
    /// Force position by one cell based on given dir.
    /// </summary>
    public void SetPosByDir(Dir dir) {
        if(tile != null) {
            transform.position = GetTilePos(dir);
            tile.Align();
        }
    }

    public void ProcessDir(Dir dir) {
        if(mCurDir != dir) {
            if(mCurState != State.Move && mCurState != State.PauseMove) {
                DoFace(dir);
            }
            else {
                mCurDir = dir;
            }
        }
        else if(mCurState != State.Move && mCurState != State.PauseMove) {
            DoCurMove();
        }
    }

    public bool CheckSolid(Dir dir) {
        bool ret = false;

        if(tile != null) {
            tk2dRuntime.TileMap.TileInfo ti = tile.GetTileInfo(mCurDir);
            if(!ignoreWalls && ti != null && ti.intVal == (int)TileType.Wall)
                ret = true;
            else {
                Vector2 checkPos = GetTilePos(dir);

                ret = solidCheck.value == 0 ? false : Physics.Raycast(new Vector3(checkPos.x, checkPos.y, -1000), Vector3.forward, Mathf.Infinity, solidCheck.value);
            }
        }

        return ret;
    }

    protected override void OnUndo(Act act, Dir dir, object dat) {
        if(act == Act.Move) {
            //finish up current move
            StopMove();

            switch(dir) {
                case Dir.North:
                    SetPosByDir(Dir.South);
                    break;

                case Dir.South:
                    SetPosByDir(Dir.North);
                    break;

                case Dir.East:
                    SetPosByDir(Dir.West);
                    break;

                case Dir.West:
                    SetPosByDir(Dir.East);
                    break;
            }

            //check if we are standing on a slow tile
            tk2dRuntime.TileMap.TileInfo tileInf = tile.tileData;
            if(tileInf != null && tileInf.intVal == (int)TileType.Slow && mSlowCounter < slowMaxCount) {
                mSlowCounter++;
            }
            else {
                mSlowCounter = 0;
            }
        }
        else if(act == Act.MoveDelayed) {
            StopMove();
            mSlowCounter=0;
        }
    }

    protected override void OnDestroy() {
        moveStartCallback = null;
        moveFinishCallback = null;

        base.OnDestroy();
    }

    protected override void Start() {
        base.Start();

    }

    protected virtual void OnMoveCellStart() {
    }

    protected virtual void OnMoveCellFinish() {
    }

    // Update is called once per frame
    protected virtual void Update() {
        switch(mCurState) {
            case State.None:
                break;

            case State.PauseMove:
                if(mMoveActive) {
                    mCurMoveTime += Time.deltaTime;
                    if(mCurMoveTime >= pauseMoveDelay) {
                        mCurMoveTime = 0.0f;
                        mCurState = State.Move;
                        ProcessMove();
                    }
                }
                else {
                    mCurState = State.None;
                }
                break;

            case State.Move:
                mCurMoveTime += Time.deltaTime;
                if(mCurMoveTime >= moveDelay) {
                    transform.position = new Vector3(mMoveEndPos.x, mMoveEndPos.y, transform.position.z);

                    if(mMoveActive) {
                        if(tile != null)
                            tile.Set(mNextCol, mNextRow);

                        DoCurMove();
                    }
                    else {
                        if(tile != null)
                            tile.Set(mNextCol, mNextRow);

                        mCurState = State.None;
                    }

                    OnMoveCellFinish();

                    if(moveFinishCallback != null)
                        moveFinishCallback(this);

                    ProcessAct(Act.MoveEnd, mCurDir, mMoveDat, false);
                }
                else {
                    Vector2 curPos = Vector2.Lerp(mMoveStartPos, mMoveEndPos, mCurMoveTime / moveDelay);
                    transform.position = new Vector3(curPos.x, curPos.y, transform.position.z);
                }
                break;

            case State.Face:
                mCurMoveTime += Time.deltaTime;
                if(mCurMoveTime >= faceDelay) {
                    if(mMoveActive) {
                        if(tile != null)
                            tile.Align();

                        DoCurMove();
                    }
                    else {
                        mCurState = State.None;
                    }
                }
                break;
        }
    }

    protected void SetCurDir(Dir toDir) {
        mCurDir = toDir;
    }

    private void DoFace(Dir toDir) {
        mCurState = State.Face;
        mCurDir = toDir;

        if(faceDelay > 0) {
            mCurMoveTime = 0.0f;
            ProcessAct(Act.Face, toDir, null, false);
        }
        else {
            DoCurMove();
        }
    }

    private void DoCurMove() {
        mCurMoveTime = 0.0f;

        if(tile != null) {
            //check if there's a wall to the next position
            if(CheckSolid(mCurDir)) {
                ProcessAct(Act.Face, mCurDir, null, false);

                mCurState = State.None;
                //sound?
            }
            else {
                //check if we are standing on a slow tile
                tk2dRuntime.TileMap.TileInfo dat = tile.tileData;
                if(dat != null && dat.intVal == (int)TileType.Slow && mSlowCounter < slowMaxCount) {
                    mSlowCounter++;

                    mMoveStartPos = transform.position;
                    mMoveEndPos = mMoveStartPos;

                    mNextCol = tile.col;
                    mNextRow = tile.row;

                    mCurState = State.Move;

                    ProcessAct(Act.MoveDelayed, mCurDir, mMoveDat, true);
                }
                else {
                    mCurState = mCurState == State.None || mCurState == State.Face || pauseMoveDelay == 0.0f ? State.Move : State.PauseMove;

                    mMoveStartPos = transform.position;
                    mMoveEndPos = mMoveStartPos;

                    tk2dTileMap map = TileInfo.instance.map;

                    float amtX = map.partitionSizeX;
                    float amtY = map.partitionSizeY;

                    mNextCol = tile.col;
                    mNextRow = tile.row;

                    switch(mCurDir) {
                        case Dir.North:
                            mMoveEndPos.y += amtY;
                            mNextRow++;
                            break;

                        case Dir.South:
                            mMoveEndPos.y -= amtY;
                            mNextRow--;
                            break;

                        case Dir.East:
                            mMoveEndPos.x += amtX;
                            mNextCol++;
                            break;

                        case Dir.West:
                            mMoveEndPos.x -= amtX;
                            mNextCol--;
                            break;
                    }

                    if(mCurState == State.Move)
                        ProcessMove();
                }
            }
        }
    }

    private void ProcessMove() {
        mSlowCounter = 0;

        ProcessAct(Act.Move, mCurDir, mMoveDat, true);

        OnMoveCellStart();

        if(moveStartCallback != null)
            moveStartCallback(this);
    }
}
