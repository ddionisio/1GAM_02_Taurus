using UnityEngine;
using System.Collections;

public class ModalHelp : UIController {
    public UIEventListener ok;

    protected override void OnActive(bool active) {
        InputManager input = Main.instance != null ? Main.instance.input : null;

        if(active) {
            if(ok.gameObject.activeInHierarchy)
                ok.onClick = OKClick;

            if(input != null) {
                input.AddButtonCall(InputAction.MenuEnter, OnInputMenu);
            }
        }
        else {
            ok.onClick = null;

            Main.instance.input.RemoveButtonCall(InputAction.MenuEnter, OnInputMenu);
        }
    }

    protected override void OnOpen() {
    }

    protected override void OnClose() {
    }

    void OnInputMenu(InputManager.Info data) {
        if(data.state == InputManager.State.Released) {
            UIModalManager.instance.ModalCloseTop();
        }
    }

    void OKClick(GameObject go) {
        UIModalManager.instance.ModalCloseTop();
    }
}
