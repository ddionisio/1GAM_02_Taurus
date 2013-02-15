using UnityEngine;
using System.Collections;

public class LevelSelectController : MonoBehaviour {
    public enum State {
        None,
        Moving
    }

    public LevelSelectNode[] nodes;
    public GameObject selector;
    public float moveDelay;

    private State mState = State.None;
    private Vector2 mStartPos;
    private Vector2 mEndPos;
    private float mCurMoveTime;
    private int mCurLevelSelect;

    void OnDestroy() {
        InputManager input = Main.instance != null ? Main.instance.input : null;
        if(input != null) {
            input.RemoveButtonCall(InputAction.MenuEnter, OnInputEnter);
            input.RemoveButtonCall(InputAction.MenuEscape, OnInputExit);
        }
    }

	// Use this for initialization
	void Start () {
        int availableLevel = 0;
        for(int i = 0; i < nodes.Length; i++) {
            LevelSelectNode node = nodes[i];

            if(LevelConfig.instance.CheckLevelUnlock(i)) {
                bool secret = LevelConfig.instance.CheckLevelSecretUnlock(i);
                node.SetState(LevelSelectNode.State.complete, secret);

                availableLevel = i + 1;
            }
            else {
                node.SetState(LevelSelectNode.State.locked, false);
                break;
            }
        }

        if(availableLevel < nodes.Length) {
            nodes[availableLevel].SetState(LevelSelectNode.State.unlocked, false);

            //set the rest as locked
            for(int j = availableLevel + 1; j < nodes.Length; j++) {
                LevelSelectNode node = nodes[j];
                node.SetState(LevelSelectNode.State.locked, false);
            }
        }
        else {
            //everything is complete
            availableLevel = nodes.Length - 1;
        }

        //set selector to available level
        Vector3 nodePos = nodes[availableLevel].transform.position;
        selector.transform.position = new Vector3(nodePos.x, nodePos.y, selector.transform.position.z);

        mCurLevelSelect = availableLevel;

        //bind input
        InputManager input = Main.instance.input;
        input.AddButtonCall(InputAction.MenuEnter, OnInputEnter);
        input.AddButtonCall(InputAction.MenuEscape, OnInputExit);
	}
	
	// Update is called once per frame
	void Update () {
        switch(mState) {
            case State.None:
                InputManager input = Main.instance.input;
                float axis = input.GetAxis(InputAction.MenuHorizontal);
                if(axis < 0.0f) {
                    MovePrev();
                }
                else if(axis > 0.0f) {
                    MoveNext();
                }
                break;

            case State.Moving:
                mCurMoveTime += Time.deltaTime;
                if(mCurMoveTime >= moveDelay) {
                    Vector3 nodePos = nodes[mCurLevelSelect].transform.position;
                    selector.transform.position = new Vector3(nodePos.x, nodePos.y, selector.transform.position.z);

                    mState = State.None;
                }
                else {
                    float t = mCurMoveTime / moveDelay;

                    Vector2 pos = Vector2.Lerp(mStartPos, mEndPos, t);
                    selector.transform.position = new Vector3(pos.x, pos.y, selector.transform.position.z);
                }
                break;
        }
	}

    void OnInputEnter(InputManager.Info data) {
        if(data.state == InputManager.State.Pressed && mState == State.None) {
            //start level
            Main.instance.sceneManager.LoadLevel(mCurLevelSelect);
        }
    }

    void OnInputExit(InputManager.Info data) {
        if(data.state == InputManager.State.Pressed) {
            //return to main
            Main.instance.sceneManager.LoadScene(Main.instance.startScene);
        }
    }

    private void MovePrev() {
        if(mCurLevelSelect > 0) {
            mCurMoveTime = 0.0f;

            mStartPos = nodes[mCurLevelSelect].transform.position;

            mCurLevelSelect--;

            mEndPos = nodes[mCurLevelSelect].transform.position;

            mState = State.Moving;
        }
    }

    private void MoveNext() {
        if(mCurLevelSelect < nodes.Length - 1) {
            mCurMoveTime = 0.0f;

            mStartPos = nodes[mCurLevelSelect].transform.position;

            mCurLevelSelect++;

            mEndPos = nodes[mCurLevelSelect].transform.position;

            mState = State.Moving;
        }
    }
}
