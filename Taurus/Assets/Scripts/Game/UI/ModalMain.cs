using UnityEngine;
using System.Collections;

public class ModalMain : UIController {
    public UIEventListener start;
    public UIEventListener howto;
    public UIEventListener options;

    protected override void OnActive(bool active) {
        if(active) {
            start.onClick = StartClick;
            howto.onClick = HowToClick;
            options.onClick = OptionsClick;
        }
        else {
            start.onClick = null;
            howto.onClick = null;
            options.onClick = null;
        }
    }

    protected override void OnOpen() {
    }

    protected override void OnClose() {
    }

    void StartClick(GameObject go) {
        UIModalManager.instance.ModalOpen(Modals.slots);
    }

    void HowToClick(GameObject go) {
        Debug.Log("how to?");
    }

    void OptionsClick(GameObject go) {
        Debug.Log("options?");
    }
}