using UnityEngine;
using System.Collections;

public class PlayerArrow : MonoBehaviour {
    public Dir dir;
    public GameObject display;

    private Player mPlayer;
    private bool mStarted = false;

    void OnEnable() {
        if(mStarted) {
            display.SetActive(UICamera.IsHighlighted(gameObject));
        }
    }

    void OnDestroy() {
        if(mPlayer != null) {
            if(mPlayer != null) {
                mPlayer.moveStartCallback -= OnMoveStart;
                mPlayer.moveFinishCallback -= OnMoveFinish;
                mPlayer.actCallback -= OnAct;
                mPlayer.undoCallback -= OnUndo;
            }
        }
    }

    // Use this for initialization
    void Start() {
        mPlayer = transform.parent.GetComponent<Player>();

        mPlayer.ApplyProperDir(ref dir);

        mPlayer.moveStartCallback += OnMoveStart;
        mPlayer.moveFinishCallback += OnMoveFinish;
        mPlayer.actCallback += OnAct;
        mPlayer.undoCallback += OnUndo;


        mStarted = true;
        display.SetActive(false);
    }

    void OnHover(bool yes) {
        display.SetActive(yes);

        if(yes) {
            ActionManager.instance.InputHover(dir);
        }
    }

    void OnClick() {
        mPlayer.HighlightBlockInFront();

        if(mPlayer.blockInFront) {
            ActionManager.instance.InputClickFirePrep();
            mPlayer.DoFire();
            ActionManager.instance.InputClickFireFinish();
        }
        else {
            ActionManager.instance.InputClick(dir);

            if(!mPlayer.CheckSolid(mPlayer.curDir)) {
                collider.enabled = false;
                display.SetActive(false);
            }
        }
    }

    void OnUIModalActive() {
        collider.enabled = false;
    }

    void OnUIModalInactive() {
        collider.enabled = true;
    }

    void OnMoveStart(ActorMove mover) {
        collider.enabled = false;
        display.SetActive(false);
    }

    void OnMoveFinish(ActorMove mover) {
        collider.enabled = true;
        OnHover(UICamera.IsHighlighted(gameObject));
    }

    void OnAct(Act act, Dir dir) {
        switch(act) {
            case Act.Cry:
            case Act.Die:
            case Act.Victory:
                collider.enabled = false;
                display.SetActive(false);
                break;
        }
    }

    void OnUndo(Act act, Dir dir) {
        collider.enabled = true;
        OnHover(UICamera.IsHighlighted(gameObject));
    }
}