using UnityEngine;
using System.Collections;

public class ActorSpriteController : MonoBehaviour {
    public Actor actor;
    public tk2dAnimatedSprite sprite;
    public bool disableAfterDieEnd = false;

    private int[] mFaceStateIds = new int[(int)Dir.NumDir-1];
    private int[] mMoveStateIds = new int[(int)Dir.NumDir-1];
    private int[] mKillStateIds = new int[(int)Dir.NumDir-1];
    private int mKillOnSpotId;

    private int mDieStateId;
    private int mCryStateId;
    private int mVictoryStateId;

    private Act mPrevAct = Act.Face;

    void OnDestroy() {
        if(actor != null) {
            actor.actCallback -= OnAct;
            actor.undoCallback -= OnUndoAct;
        }

        if(sprite != null) {
            sprite.animationCompleteDelegate -= AnimationComplete;
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
        mKillOnSpotId = sprite.GetClipIdByName(Act.Kill.ToString());
    }

    void Awake() {
        actor.actCallback += OnAct;
        actor.undoCallback += OnUndoAct;

        sprite.animationCompleteDelegate += AnimationComplete;
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

            case Act.MoveEnd:
                if(mPrevAct == Act.Move || mPrevAct == Act.MoveDelayed)
                    playId = mFaceStateIds[dirInd];
                break;

            case Act.MoveDelayed:
            case Act.Face:
            case Act.Fire:
                if(mPrevAct != Act.Kill)
                    playId = mFaceStateIds[dirInd];
                break;

            case Act.Kill:
                playId = dir == Dir.NumDir ? mKillOnSpotId : mKillStateIds[dirInd];
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

        mPrevAct = act;
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
                playId = mFaceStateIds[(int)Dir.South];

                if(disableAfterDieEnd) {
                    actor.gameObject.SetActive(true);
                }
                break;

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

        mPrevAct = Act.Face;
    }

    void AnimationComplete(tk2dAnimatedSprite sprite, int clipId) {
        if(clipId == mDieStateId) {
            if(disableAfterDieEnd) {
                actor.gameObject.SetActive(false);
            }
        }
    }
}
