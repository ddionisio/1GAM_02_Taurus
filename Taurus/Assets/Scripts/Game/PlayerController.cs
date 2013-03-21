using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    private static Player[] mPlayers;

    private bool mVictory = false;
    private bool mSecretMade = false;
    private int mSecretUndoCount = -1; //this is the last count of undo when secret was made

    public static Player[] players { get { return mPlayers; } }

    /// <summary>
    /// Set given player to die, others will cry.
    /// </summary>
    public static void KillPlayer(Player p) {
        p.Die();

        //tell other players to cry
        foreach(Player other in mPlayers) {
            if(other != p) {
                other.Cry();
            }
        }

        //open ui thing
        /*if(UIModalManager.instance.ModalGetTop() != Modals.gameover) {
            UIModalManager.instance.ModalOpen(Modals.gameover);
        }*/
    }

    public void Undo() {
        //undo the secret that was made
        if(mSecretMade && mSecretUndoCount == ActionManager.instance.undoCount) {
            mSecretMade = false;
            Debug.Log("secret undo");
        }

        ActionManager.instance.InputUndo();
    }
    
    void OnDestroy() {
        //remove input
        InputSetup(false);

        mPlayers = null;
    }

    void Awake() {
        GameObject[] playersGo = GameObject.FindGameObjectsWithTag(Layers.tagPlayer);
        mPlayers = new Player[playersGo.Length];
        for(int i = 0; i < playersGo.Length; i++) {
            mPlayers[i] = playersGo[i].GetComponent<Player>();
            mPlayers[i].moveFinishCallback += OnPlayerMoveFinish;
        }
    }

    // Use this for initialization
    void Start() {
        //setup input
        InputSetup(true);
    }

#if UNITY_EDITOR
    void Update() {
        if(Input.GetKey(KeyCode.LeftControl)) {
            if(Input.GetKeyDown(KeyCode.F5)) {
                CheatVictory(false);
            }
            else if(Input.GetKeyDown(KeyCode.F6)) {
                CheatVictory(true);
            }
        }
    }
#endif

    void OnUIModalActive() {
        InputSetup(false);
    }

    void OnUIModalInactive() {
        InputSetup(true);
    }

    void OnInputUp(InputManager.Info data) {
        if(data.state == InputManager.State.Pressed) {
            ActionManager.instance.InputAct(InputAction.Up, true); 
        }
        else if(data.state == InputManager.State.Released) {
            ActionManager.instance.InputAct(InputAction.Up, false); 
        }
    }

    void OnInputDown(InputManager.Info data) {
        if(data.state == InputManager.State.Pressed) {
            ActionManager.instance.InputAct(InputAction.Down, true);
        }
        else if(data.state == InputManager.State.Released) {
            ActionManager.instance.InputAct(InputAction.Down, false);
        }
    }

    void OnInputLeft(InputManager.Info data) {
        if(data.state == InputManager.State.Pressed) {
            ActionManager.instance.InputAct(InputAction.Left, true);
        }
        else if(data.state == InputManager.State.Released) {
            ActionManager.instance.InputAct(InputAction.Left, false);
        }
    }

    void OnInputRight(InputManager.Info data) {
        if(data.state == InputManager.State.Pressed) {
            ActionManager.instance.InputAct(InputAction.Right, true);
        }
        else if(data.state == InputManager.State.Released) {
            ActionManager.instance.InputAct(InputAction.Right, false);
        }
    }

    void OnInputFire(InputManager.Info data) {
        if(data.state == InputManager.State.Pressed) {
            ActionManager.instance.InputAct(InputAction.Fire, true);
        }
        else if(data.state == InputManager.State.Released) {
            ActionManager.instance.InputAct(InputAction.Fire, false);
        }
    }

    void OnInputUndo(InputManager.Info data) {
        if(data.state == InputManager.State.Pressed) {
            Undo();
        }
    }

    void OnInputMenu(InputManager.Info data) {
        if(data.state == InputManager.State.Pressed) {
            UIModalManager.instance.ModalOpen(Modals.pause);
        }
    }

    void OnPlayerMoveFinish(ActorMove mover) {
        int numDoneMove = 0;
        foreach(Player p in mPlayers) {
            if(p.state == ActorMove.State.None)
                numDoneMove++;
        }

        if(numDoneMove == mPlayers.Length)
            CheckVictory();
    }
        
    private void InputSetup(bool attach) {
        InputManager input = Main.instance != null ? Main.instance.input : null;

        if(input != null) {
            if(attach) {
                input.AddButtonCall(InputAction.Up, OnInputUp);
                input.AddButtonCall(InputAction.Down, OnInputDown);
                input.AddButtonCall(InputAction.Left, OnInputLeft);
                input.AddButtonCall(InputAction.Right, OnInputRight);
                input.AddButtonCall(InputAction.Fire, OnInputFire);
                input.AddButtonCall(InputAction.Undo, OnInputUndo);

                input.AddButtonCall(InputAction.Menu, OnInputMenu);
            }
            else {
                input.RemoveButtonCall(InputAction.Up, OnInputUp);
                input.RemoveButtonCall(InputAction.Down, OnInputDown);
                input.RemoveButtonCall(InputAction.Left, OnInputLeft);
                input.RemoveButtonCall(InputAction.Right, OnInputRight);
                input.RemoveButtonCall(InputAction.Fire, OnInputFire);
                input.RemoveButtonCall(InputAction.Undo, OnInputUndo);

                input.RemoveButtonCall(InputAction.Menu, OnInputMenu);
            }
        }
    }

    private void CheatVictory(bool secretInclude) {
        LevelConfig.instance.SaveLevelUnlock(Main.instance.sceneManager.curLevel, secretInclude);

        //open ui thing
        if(UIModalManager.instance.ModalGetTop() != Modals.victory) {
            UIModalManager.instance.ModalOpen(Modals.victory);
        }
    }

    private void CheckVictory() {
        int numInGoal = 0;
        int numSecret = 0;
        foreach(Player p in mPlayers) {
            if(p.onGoal)
                numInGoal++;

            if(p.secretTouched)
                numSecret++;
        }

        if(!mSecretMade && numSecret == mPlayers.Length) {
            mSecretMade = true;
            mSecretUndoCount = ActionManager.instance.inputDownCounter == 0 ? ActionManager.instance.undoCount : ActionManager.instance.undoCount + 1;
            Debug.Log("secret!");

            foreach(Player p in mPlayers) {
                if(p.onSecretMadeObject != null)
                    p.onSecretMadeObject.SetActive(true);
            }
        }

        //win?
        mVictory = numInGoal == mPlayers.Length;

        if(mVictory) {
            Debug.Log("victory");

            if(mSecretMade)
                Debug.Log("secret made");

            foreach(Player p in mPlayers) {
                p.Victory();
            }

            //save current level progress (note: make sure the level was loaded via scene manager prior to this!)
            LevelConfig.instance.SaveLevelUnlock(Main.instance.sceneManager.curLevel, mSecretMade);

            //activate fan-fare crap

            //open ui thing
            if(UIModalManager.instance.ModalGetTop() != Modals.victory) {
                UIModalManager.instance.ModalOpen(Modals.victory);
            }
        }
    }

}
