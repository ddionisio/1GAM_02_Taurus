using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    private static Player[] mPlayers;

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
        InputManager input = Main.instance != null ? Main.instance.input : null;

        if(input != null) {
            input.RemoveButtonCall(InputAction.Up, OnInputUp);
            input.RemoveButtonCall(InputAction.Down, OnInputDown);
            input.RemoveButtonCall(InputAction.Left, OnInputLeft);
            input.RemoveButtonCall(InputAction.Right, OnInputRight);
            input.RemoveButtonCall(InputAction.Fire, OnInputFire);
            input.RemoveButtonCall(InputAction.Undo, OnInputUndo);
            input.RemoveButtonCall(InputAction.Menu, OnInputMenu);
        }

        mPlayers = null;
    }

    void Awake() {
        GameObject[] playersGo = GameObject.FindGameObjectsWithTag(Layers.tagPlayer);
        mPlayers = new Player[playersGo.Length];
        for(int i = 0; i < playersGo.Length; i++) {
            mPlayers[i] = playersGo[i].GetComponent<Player>();
        }
    }

    // Use this for initialization
    void Start() {
        //setup input
        InputManager input = Main.instance.input;

        input.AddButtonCall(InputAction.Up, OnInputUp);
        input.AddButtonCall(InputAction.Down, OnInputDown);
        input.AddButtonCall(InputAction.Left, OnInputLeft);
        input.AddButtonCall(InputAction.Right, OnInputRight);
        input.AddButtonCall(InputAction.Fire, OnInputFire);
        input.AddButtonCall(InputAction.Undo, OnInputUndo);
        input.AddButtonCall(InputAction.Menu, OnInputMenu);
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
    }
}
