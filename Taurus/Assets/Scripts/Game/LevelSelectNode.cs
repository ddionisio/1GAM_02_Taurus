using UnityEngine;
using System.Collections;

public class LevelSelectNode : MonoBehaviour {
    public delegate void OnClickCall();

    public enum State {
        locked,
        unlocked,
        complete
    }

    public tk2dBaseSprite image;
    
    public GameObject highlight;
    public GameObject secret;
    public GameObject complete;

    public GameObject cursorLeft;
    public GameObject cursorRight;

    public Color lockedColor;

    public OnClickCall cursorLeftOnClick;
    public OnClickCall cursorRightOnClick;
    public OnClickCall levelOnClick;
    
    private State mCurState;

    private tk2dBaseSprite[] mCompleteSprites;

    public State curState { get { return mCurState; } }

    public bool highlightActive {
        get { return highlight != null ? highlight.activeSelf : false; }

        set {
            if(highlight != null)
                highlight.SetActive(value);
        }
    }

    public void SetState(State state, bool secretUnlocked) {
        mCurState = state;

        Color imageClr = Color.white;
        bool completeActive = false;
                
        switch(state) {
            case State.unlocked:
                break;

            case State.locked:
                imageClr = lockedColor;
                break;

            case State.complete:
                completeActive = true;
                break;
        }

        image.color = imageClr;

        if(mCompleteSprites != null && !completeActive) {
            foreach(tk2dBaseSprite spr in mCompleteSprites)
                spr.color = lockedColor;
        }

        if(secret != null)
            secret.SetActive(secretUnlocked);
    }

    public void SetCursor(bool left, bool right) {
        if(cursorLeft != null)
            cursorLeft.SetActive(left);
        if(cursorRight != null)
            cursorRight.SetActive(right);
    }

    void Awake() {
        UIEventListener.Get(image.gameObject).onClick = OnLevelClick;

        if(highlight != null)
            highlight.SetActive(false);
        if(secret != null)
            secret.SetActive(false);
        if(cursorLeft != null) {
            cursorLeft.SetActive(false);
            UIEventListener.Get(cursorLeft).onClick = OnLeftClick;
        }
        if(cursorRight != null) {
            cursorRight.SetActive(false);
            UIEventListener.Get(cursorRight).onClick = OnRightClick;
        }

        if(complete != null)
            mCompleteSprites = complete.GetComponentsInChildren<tk2dBaseSprite>();
    }

    void OnLeftClick(GameObject go) {
        if(cursorLeftOnClick != null)
            cursorLeftOnClick();
    }

    void OnRightClick(GameObject go) {
        if(cursorRightOnClick != null)
            cursorRightOnClick();
    }

    void OnLevelClick(GameObject go) {
        if(levelOnClick != null)
            levelOnClick();
    }
}
