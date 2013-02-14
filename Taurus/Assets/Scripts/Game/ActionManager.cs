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
    public delegate void OnAct(Act act, Dir dir);

    public bool inputListen = true;
    public bool undo = true;

    public event OnAct actCallback;
    public event OnAct undoCallback;

    private TileAlign mTileAlign;

    public TileAlign tile { get { return mTileAlign; } }
        
    //Interfaces:

    /// <summary>
    /// Called by ActionManager when undo'ing an action.
    /// </summary>
    public void Undo(Act act, Dir dir) {
        if(undo) {
            OnUndo(act, dir);

            if(undoCallback != null)
                undoCallback(act, dir);
        }
    }
        
    /// <summary>
    /// Call this to process an action. This will send out the event to actCallback and optionally add to action undo.
    /// </summary>
    protected void ProcessAct(Act act, Dir dir, bool canUndo) {
        if(canUndo) {
            ActionManager.instance.ActAdd(this, act, dir);
        }

        if(actCallback != null)
            actCallback(act, dir);
    }

    //Implements:

    /// <summary>
    /// Called by Undo to process what needs to be undone.
    /// </summary>
    protected abstract void OnUndo(Act act, Dir dir);

    /// <summary>
    /// Called by ActionManager when input of act and dir occurs. Actor will act accordingly and call ActionManager.ActAdd if needed.
    /// </summary>
    protected abstract void OnInputAct(InputAction input, bool down);

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

        public ActData(Actor aActor, Act aAct, Dir aDir) {
            actor = aActor;
            act = aAct;
            dir = aDir;
        }
    }

    private static ActionManager mInstance = null;

    private List<List<ActData>> mActs = new List<List<ActData>>(maxStack);

    private List<ActData> mCurActProcess = null;

    public static ActionManager instance { get { return mInstance; } }

    //this will notify listeners about an act performed via input
    public void InputAct(InputAction input, bool down) {
        if(down) {
            if(mCurActProcess == null)
                mCurActProcess = new List<ActData>();
        }
        else {
            if(mCurActProcess != null) {
                if(mCurActProcess.Count > 0) {
                    if(mActs.Count == maxStack) {
                        mActs.RemoveAt(0);
                    }

                    mActs.Add(mCurActProcess);
                }

                mCurActProcess = null;
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

                foreach(ActData act in acts) {
                    if(act.actor != null) {
                        act.actor.Undo(act.act, act.dir);
                    }
                }

                mActs.RemoveAt(mActs.Count - 1);

                Debug.Log("num act lists: " + mActs.Count);
            }
        }
    }

    //add to the current stack
    public void ActAdd(Actor actor, Act act, Dir dir) {
        if(mCurActProcess != null) {
            Debug.Log("process: " + act + " dir: " + dir + " count: " + mCurActProcess.Count);
            mCurActProcess.Add(new ActData(actor, act, dir));
        }
        else if(mActs.Count > 0) {
            List<ActData> acts = mActs[mActs.Count - 1];
            Debug.Log("stack process: " + act + " dir: " + dir + " count: " + acts.Count);
            acts.Add(new ActData(actor, act, dir));
        }
    }

    void OnDestroy() {
        mInstance = null;
    }

    void Awake() {
        mInstance = this;
    }
}
