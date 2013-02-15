using UnityEngine;
using System.Collections;

public class LevelSelectNode : MonoBehaviour {
    public enum State {
        locked,
        unlocked,
        complete
    }

    public tk2dAnimatedSprite sprite;
    public GameObject nextNode;
    public GameObject secretGo;
    public Color nextNodeLockColor;

    private tk2dBaseSprite[] mNodeSprites = null;
    private State mCurState;

    public State curState { get { return mCurState; } }

    public void SetState(State state, bool secretUnlocked) {
        mCurState = state;

        sprite.Play(state.ToString());

        secretGo.SetActive(secretUnlocked);

        if(mNodeSprites != null) {
            if(state == State.complete) {
                foreach(tk2dBaseSprite spr in mNodeSprites)
                    spr.color = Color.white;
            }
            else {
                foreach(tk2dBaseSprite spr in mNodeSprites)
                    spr.color = nextNodeLockColor;
            }
        }
    }

    void Awake() {
        if(nextNode != null)
            mNodeSprites = nextNode.GetComponentsInChildren<tk2dBaseSprite>();
    }

}
