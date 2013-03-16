using UnityEngine;
using System.Collections;

public class ModalVictory : UIController {
    public const string bablePrefix = "bable";
    public const int bableMax = 8;

    public UILabel bable;

    protected override void OnActive(bool active) {
        InputManager input = Main.instance.input;

        if(active) {
            input.AddButtonCall(InputAction.MenuEscape, OnInputEsc);
            input.AddButtonCall(InputAction.MenuEnter, OnInputProceed);
        }
        else {
            input.RemoveButtonCall(InputAction.MenuEscape, OnInputEsc);
            input.RemoveButtonCall(InputAction.MenuEnter, OnInputProceed);
        }
    }

    protected override void OnOpen() {
        if(bable != null) {
            bable.text = GameLocalize.GetText(bablePrefix + Random.Range(0, bableMax));
        }
    }

    protected override void OnClose() {
    }

    void OnInputEsc(InputManager.Info data) {
        if(data.state == InputManager.State.Pressed)
            UIModalManager.instance.ModalOpen(Modals.pause);
    }

    void OnInputProceed(InputManager.Info data) {
        if(data.state == InputManager.State.Pressed)
            LevelConfig.instance.LoadLevel(Main.instance.sceneManager.curLevel + 1);
    }
}