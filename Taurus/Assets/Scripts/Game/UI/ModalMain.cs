using UnityEngine;
using System.Collections;

public class ModalMain : UIController {
    public UIEventListener start;
    public UIEventListener howto;
    public UIEventListener options;
    public UIEventListener credits;

    protected override void OnActive(bool active) {
        if(active) {
            start.onClick = StartClick;
            howto.onClick = HowToClick;
            options.onClick = OptionsClick;
            credits.onClick = CreditsClick;
        }
        else {
            start.onClick = null;
            howto.onClick = null;
            options.onClick = null;
            credits.onClick = null;
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
        UIModalManager.instance.ModalOpen(Modals.help);
    }

    void OptionsClick(GameObject go) {
        UIModalManager.instance.ModalOpen(Modals.options);
    }

    void CreditsClick(GameObject go) {
        UIModalManager.instance.ModalOpen(Modals.credits);
    }
}