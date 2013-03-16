using UnityEngine;
using System.Collections;

public class ModalGameOver : UIController {

    protected override void OnActive(bool active) {
        InputManager input = Main.instance.input;

        if(active) {
            input.AddButtonCall(InputAction.MenuEscape, OnInputEsc);
            input.AddButtonCall(InputAction.Undo, OnInputUndo);
        }
        else {
            input.RemoveButtonCall(InputAction.MenuEscape, OnInputEsc);
            input.RemoveButtonCall(InputAction.Undo, OnInputUndo);
        }
    }

    protected override void OnOpen() {
    }

    protected override void OnClose() {
    }

    void OnInputEsc(InputManager.Info data) {
        if(data.state == InputManager.State.Pressed)
            UIModalManager.instance.ModalOpen(Modals.pause);
    }

    void OnInputUndo(InputManager.Info data) {
        if(data.state == InputManager.State.Pressed) {
            GameObject go = GameObject.FindGameObjectWithTag(Layers.tagController);
            PlayerController pc = go.GetComponent<PlayerController>();
            pc.Undo();

            UIModalManager.instance.ModalCloseTop();
        }
    }
}