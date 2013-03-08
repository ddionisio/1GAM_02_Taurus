using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class LevelSelectController : MonoBehaviour,  IComparer<LevelSelectNode> {
    public enum State {
        None,
        Moving
    }

    public GameObject nodesHolder;
    public GameObject selector;
    public float moveDelay;

    private State mState = State.None;
    private Vector2 mStartPos;
    private Vector2 mEndPos;
    private float mCurMoveTime;
    private int mCurLevelSelect;

    private LevelSelectNode[] mNodes;

    void OnDestroy() {
        InputManager input = Main.instance != null ? Main.instance.input : null;
        if(input != null) {
            input.RemoveButtonCall(InputAction.MenuEnter, OnInputEnter);
            input.RemoveButtonCall(InputAction.MenuEscape, OnInputExit);
        }
    }

    public int Compare(LevelSelectNode x, LevelSelectNode y) {
        string xNumStr = Regex.Match(x.gameObject.name, @"\d+").Value;
        string yNumStr = Regex.Match(y.gameObject.name, @"\d+").Value;

        return int.Parse(xNumStr) - int.Parse(yNumStr);
    }

    void Awake() {
        mNodes = nodesHolder.GetComponentsInChildren<LevelSelectNode>(true);
        System.Array.Sort<LevelSelectNode>(mNodes, this);
    }

	// Use this for initialization
	void Start () {
        int availableLevel = 0;
        for(int i = 0; i < mNodes.Length; i++) {
            LevelSelectNode node = mNodes[i];

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

        if(availableLevel < mNodes.Length) {
            mNodes[availableLevel].SetState(LevelSelectNode.State.unlocked, false);

            //set the rest as locked
            for(int j = availableLevel + 1; j < mNodes.Length; j++) {
                LevelSelectNode node = mNodes[j];
                node.SetState(LevelSelectNode.State.locked, false);
            }
        }
        else {
            //everything is complete
            availableLevel = mNodes.Length - 1;
        }

        //set selector to available level
        Vector3 nodePos = mNodes[availableLevel].transform.position;
        selector.transform.position = new Vector3(nodePos.x, nodePos.y, selector.transform.position.z);

        mCurLevelSelect = availableLevel;

        mNodes[mCurLevelSelect].highlightActive = true;
        mNodes[mCurLevelSelect].SetCursor(mCurLevelSelect > 0, false);

        //bind input
        InputManager input = Main.instance.input;
        input.AddButtonCall(InputAction.MenuEnter, OnInputEnter);
        input.AddButtonCall(InputAction.MenuEscape, OnInputExit);

        //preserve the music play throughout
        Object.DontDestroyOnLoad(MusicManager.instance.gameObject);
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
                    Vector3 nodePos = mNodes[mCurLevelSelect].transform.position;
                    selector.transform.position = new Vector3(nodePos.x, nodePos.y, selector.transform.position.z);

                    mNodes[mCurLevelSelect].highlightActive = true;
                    mNodes[mCurLevelSelect].SetCursor(
                        mCurLevelSelect > 0 && mNodes[mCurLevelSelect - 1].curState != LevelSelectNode.State.locked,
                        mCurLevelSelect < mNodes.Length - 1 && mNodes[mCurLevelSelect + 1].curState != LevelSelectNode.State.locked);

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
            ReturnToMain();
        }
    }

    private void ReturnToMain() {
        Object.Destroy(MusicManager.instance.gameObject);
        Main.instance.sceneManager.LoadScene(Main.instance.startScene);
    }

    private void MovePrev() {
        if(mCurLevelSelect > 0) {
            mNodes[mCurLevelSelect].highlightActive = false;
            mNodes[mCurLevelSelect].SetCursor(false, false);

            mCurMoveTime = 0.0f;

            mStartPos = mNodes[mCurLevelSelect].transform.position;

            mCurLevelSelect--;

            mEndPos = mNodes[mCurLevelSelect].transform.position;

            mState = State.Moving;
        }
    }

    private void MoveNext() {
        if(mCurLevelSelect < mNodes.Length - 1 && mNodes[mCurLevelSelect+1].curState != LevelSelectNode.State.locked) {
            mNodes[mCurLevelSelect].highlightActive = false;
            mNodes[mCurLevelSelect].SetCursor(false, false);

            mCurMoveTime = 0.0f;

            mStartPos = mNodes[mCurLevelSelect].transform.position;

            mCurLevelSelect++;

            mEndPos = mNodes[mCurLevelSelect].transform.position;

            mState = State.Moving;
        }
    }
}
