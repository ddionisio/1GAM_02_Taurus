using UnityEngine;
using System.Collections;

public class ActorSpriteController : MonoBehaviour {
    public Actor actor;
    public tk2dAnimatedSprite sprite;

    private int[] mFaceStateIds = new int[(int)Dir.NumDir-1];
    private int[] mMoveStateIds = new int[(int)Dir.NumDir-1];
    private int[] mKillStateIds = new int[(int)Dir.NumDir-1];

    private int mDieStateId;
    private int mCryStateId;
    private int mVictoryStateId;

    void OnDestroy() {
        if(actor != null) {
            actor.actCallback -= OnAct;
            actor.undoCallback -= OnUndoAct;
        }
    }

    // Use this for initialization
    void Start() {
        //NOTE: assumes East is the last in the enum...
        for(int i = 0, numDir = (int)Dir.NumDir-1; i < numDir; i++) {
                mFaceStateIds[i] = sprite.GetClipIdByName(Act.Face.ToString() + ((Dir)i).ToString());
                mMoveStateIds[i] = sprite.GetClipIdByName(Act.Move.ToString() + ((Dir)i).ToString());
                mKillStateIds[i] = sprite.GetClipIdByName(Act.Kill.ToString() + ((Dir)i).ToString());
        }

        mDieStateId = sprite.GetClipIdByName(Act.Die.ToString());
        mCryStateId = sprite.GetClipIdByName(Act.Cry.ToString());
        mVictoryStateId = sprite.GetClipIdByName(Act.Victory.ToString());
    }

    void Awake() {
        actor.actCallback += OnAct;
        actor.undoCallback += OnUndoAct;
    }

    void OnAct(Act act, Dir dir) {
        int playId = -1;
        bool hflip;
        int dirInd;

        if(dir == Dir.West) {
            hflip = true;
            dirInd = (int)Dir.East;
        }
        else {
            hflip = false;
            dirInd = (int)dir;
        }

        switch(act) {
            case Act.Move:
                playId = mMoveStateIds[dirInd];
                break;

            case Act.MoveDelayed:
            case Act.MoveEnd:
            case Act.Face:
            case Act.Fire:
                playId = mFaceStateIds[dirInd];
                break;

            case Act.Kill:
                playId = mKillStateIds[dirInd];
                break;

            case Act.Die:
                playId = mDieStateId;
                break;

            case Act.Cry:
                playId = mCryStateId;
                break;

            case Act.Victory:
                playId = mVictoryStateId;
                break;
        }

        if(playId != -1) {
            sprite.Play(playId);

            Vector3 s = sprite.scale;
            s.x = hflip ? -Mathf.Abs(s.x) : Mathf.Abs(s.x);
            sprite.scale = s;
        }
    }

    void OnUndoAct(Act act, Dir dir) {
        int playId = -1;
        bool hflip;
        int dirInd;

        if(dir == Dir.West) {
            hflip = true;
            dirInd = (int)Dir.East;
        }
        else {
            hflip = false;
            dirInd = (int)dir;
        }

        switch(act) {
            case Act.Move:
            case Act.MoveEnd:
            case Act.Kill:
                playId = mFaceStateIds[dirInd];
                break;

            case Act.Die:
            case Act.Cry:
                playId = mFaceStateIds[(int)Dir.South];
                break;
        }

        if(playId != -1) {
            sprite.Play(playId);

            Vector3 s = sprite.scale;
            s.x = hflip ? -Mathf.Abs(s.x) : Mathf.Abs(s.x);
            sprite.scale = s;
        }
    }
}
