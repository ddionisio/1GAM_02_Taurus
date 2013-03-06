using UnityEngine;
using System.Collections;

public class ModalHelp : UIController {
    public UIEventListener ok;

    protected override void OnActive(bool active) {
        if(active) {
            ok.onClick = OKClick;
        }
        else {
            ok.onClick = null;
        }
    }

    protected override void OnOpen() {
    }

    protected override void OnClose() {
    }

    void OKClick(GameObject go) {
        UIModalManager.instance.ModalCloseTop();
    }
}
