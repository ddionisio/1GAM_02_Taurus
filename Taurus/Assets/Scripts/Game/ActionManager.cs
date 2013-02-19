using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Act {
    Face,
    Move,
    Fire,
    Kill,
    Die,
    Cry,
    Teleport,
    Victory
}

public enum Dir {
    North,
    South,
    East,
    West,

    NumDir
}

public abstract class Actor : MonoBehaviour {
    public delegate void ActCallback(Act act, Dir dir);

    public bool inputListen = true;
    public bool undo = true;

    public event ActCallback actCallback;
    public event ActCallback undoCallback;

    private TileAlign mTileAlign;

    public TileAlign tile { get { return mTileAlign; } }
        
    //Interfaces:

    /// <summary>
    /// Called by ActionManager when undo'ing an action.
    /// </summary>
    public void Undo(Act act, Dir dir, object dat) {
        if(undo) {
            OnUndo(act, dir, dat);

            if(undoCallback != null)
                undoCallback(act, dir);
        }
    }
        
    /// <summary>
    /// Call this to process an action. This will send out the event to actCallback and optionally add to action undo. Returns act index
    /// </summary>
    protected void ProcessAct(Act act, Dir dir, object dat, bool canUndo) {
        if(canUndo) {
            ActionManager.instance.ActAdd(this, act, dir, dat);
        }

        OnAct(act, dir);

        if(actCallback != null)
            actCallback(act, dir);
    }

    //Implements:

    protected virtual void OnAct(Act act, Dir dir) {
    }

    /// <summary>
    /// Called by Undo to process what needs to be undone.
    /// </summary>
    protected abstract void OnUndo(Act act, Dir dir, object dat);

    /// <summary>
    /// Called by ActionManager when input of act and dir occurs. Actor will act accordingly and call ActionManager.ActAdd if needed.
    /// </summary>
    protected virtual void OnInputAct(InputAction input, bool down) {
    }

    protected virtual void OnDestroy() {
        if(inputListen && ActionManager.instance != null) {
            ActionManager.instance.actCallback -= OnInputAct;
        }
    }

    protected virtual void Awake() {
        mTileAlign = GetComponent<TileAlign>();
    }

    protected virtual void Start() {
        //add as listener in action manager
        if(inputListen)
            ActionManager.instance.actCallback += OnInputAct;
    }
}

public class ActionManager : MonoBehaviour {
    public const int maxStack = 128;

    public delegate void InputCallback(InputAction input, bool down);

    public event InputCallback actCallback;

    private struct ActData {
        public Actor actor;
        public Act act;
        public Dir dir;
        public object dat;

        public ActData(Actor aActor, Act aAct, Dir aDir, object aDat) {
            actor = aActor;
            act = aAct;
            dir = aDir;
            dat = aDat;
        }
    }

    private static ActionManager mInstance = null;

    private List<List<ActData>> mActs = new List<List<ActData>>(maxStack);

    private List<ActData> mCurActProcess = null;

    private int mInputDownCounter = 0;

    public static ActionManager instance { get { return mInstance; } }

    public int undoCount { get { return mActs.Count; } }
    public int inputDownCounter { get { return mInputDownCounter; } }

    //this will notify listeners about an act performed via input
    public void InputAct(InputAction input, bool down) {
        if(down) {
            if(mCurActProcess == null)
                mCurActProcess = new List<ActData>();

            mInputDownCounter++;
        }
        else {
            mInputDownCounter--;
            if(mInputDownCounter <= 0) {
                if(mCurActProcess != null) {
                    if(mCurActProcess.Count > 0) {
                        if(mActs.Count == maxStack) {
                            mActs.RemoveAt(0);
                        }

                        mActs.Add(mCurActProcess);
                    }

                    mCurActProcess = null;
                }

                mInputDownCounter = 0;
            }
        }
        
        //these would call ActAdd depending on 'act'
        if(actCallback != null) {
            actCallback(input, down);
        }
    }

    //notify all actors in acts to undo their action
    public void InputUndo() {
        if(mCurActProcess == null) {
            if(mActs.Count > 0) {
                List<ActData> acts = mActs[mActs.Count - 1];

                Debug.Log("num undo acts: " + acts.Count);

                for(int i = acts.Count-1; i >= 0; i--) {
                    ActData act = acts[i];
                    if(act.actor != null) {
                        act.actor.Undo(act.act, act.dir, act.dat);
                    }
                }

                mActs.RemoveAt(mActs.Count - 1);

                Debug.Log("num act lists: " + mActs.Count);
            }
        }
    }

    //add to the current stack
    public void ActAdd(Actor actor, Act act, Dir dir, object dat) {
        if(mCurActProcess != null) {
            Debug.Log("process: " + act + " dir: " + dir + " count: " + mCurActProcess.Count);
            mCurActProcess.Add(new ActData(actor, act, dir, dat));
        }
        else if(mActs.Count > 0) {
            List<ActData> acts = mActs[mActs.Count - 1];
            Debug.Log("stack process: " + act + " dir: " + dir + " count: " + acts.Count);
            acts.Add(new ActData(actor, act, dir, dat));
        }
    }

    void OnDestroy() {
        mInstance = null;
    }

    void Awake() {
        mInstance = this;
    }
}
