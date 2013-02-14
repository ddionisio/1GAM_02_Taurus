using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    public const string levelVictoryPrefix = "levelwin";

    private static Player[] mPlayers;

    private bool mVictory = false;

    public static Player[] players { get { return mPlayers; } }

    /// <summary>
    /// Set given player to die, others will cry.
    /// </summary>
    public static void KillPlayer(Player p, Dir fromDir) {
        p.Die(fromDir);

        //tell other players to cry
        foreach(Player other in mPlayers) {
            if(other != p) {
                other.Cry();
            }
        }
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
            ActionManager.instance.InputUndo();
        }
    }

    void OnInputMenu(InputManager.Info data) {
        if(data.state == InputManager.State.Pressed) {
            UIModalManager.instance.ModalOpen(Modals.pause);
        }
    }

    void OnInputLevelProgress(InputManager.Info data) {
        if(data.state == InputManager.State.Pressed) {
            DoNextLevel();
        }
    }

    void OnPlayerMoveFinish(ActorMove mover) {
        CheckVictory();
    }
        
    private void InputSetup(bool attach) {
        InputManager input = Main.instance != null ? Main.instance.input : null;

        if(input != null) {
            if(attach) {
                if(!mVictory) {
                    input.AddButtonCall(InputAction.Up, OnInputUp);
                    input.AddButtonCall(InputAction.Down, OnInputDown);
                    input.AddButtonCall(InputAction.Left, OnInputLeft);
                    input.AddButtonCall(InputAction.Right, OnInputRight);
                    input.AddButtonCall(InputAction.Fire, OnInputFire);
                    input.AddButtonCall(InputAction.Undo, OnInputUndo);
                }
                else {
                    input.AddButtonCall(InputAction.MenuEnter, OnInputLevelProgress);
                }

                input.AddButtonCall(InputAction.Menu, OnInputMenu);
            }
            else {
                if(!mVictory) {
                    input.RemoveButtonCall(InputAction.Up, OnInputUp);
                    input.RemoveButtonCall(InputAction.Down, OnInputDown);
                    input.RemoveButtonCall(InputAction.Left, OnInputLeft);
                    input.RemoveButtonCall(InputAction.Right, OnInputRight);
                    input.RemoveButtonCall(InputAction.Fire, OnInputFire);
                    input.RemoveButtonCall(InputAction.Undo, OnInputUndo);
                }
                else {
                    input.RemoveButtonCall(InputAction.MenuEnter, OnInputLevelProgress);
                }

                input.RemoveButtonCall(InputAction.Menu, OnInputMenu);
                input.RemoveButtonCall(InputAction.MenuEnter, OnInputLevelProgress);
            }
        }
    }

    private void CheckVictory() {
        int numInGoal = 0;
        foreach(Player p in mPlayers) {
            if(p.onGoal)
                numInGoal++;
        }

        //win?
        mVictory = numInGoal == mPlayers.Length;

        if(mVictory) {
            Debug.Log("victory");

            InputManager input = Main.instance != null ? Main.instance.input : null;

            if(input != null) {
                input.RemoveButtonCall(InputAction.Up, OnInputUp);
                input.RemoveButtonCall(InputAction.Down, OnInputDown);
                input.RemoveButtonCall(InputAction.Left, OnInputLeft);
                input.RemoveButtonCall(InputAction.Right, OnInputRight);
                input.RemoveButtonCall(InputAction.Fire, OnInputFire);
                input.RemoveButtonCall(InputAction.Undo, OnInputUndo);

                input.AddButtonCall(InputAction.MenuEnter, OnInputLevelProgress);
            }

            foreach(Player p in mPlayers) {
                p.Victory();
            }

            //save current level progress (note: make sure the level was loaded via scene manager prior to this!)
            LevelConfig.instance.SaveLevelUnlock(Main.instance.sceneManager.curLevel);

            //activate fan-fare crap
        }
    }

    private void DoNextLevel() {
        LevelConfig.instance.LoadLevel(Main.instance.sceneManager.curLevel + 1);
    }
}
