using UnityEngine;
using System.Collections;

public class ModalLevelSelect : UIController {
    public UIEventListener options;
    public UIEventListener exit;

    protected override void OnActive(bool active) {
        if(active) {
            options.onClick = OptionsClick;
            exit.onClick = ExitClick;
        }
        else {
            options.onClick = null;
            exit.onClick = null;
        }
    }

    protected override void OnOpen() {
    }

    protected override void OnClose() {
    }

    void OptionsClick(GameObject go) {
        UIModalManager.instance.ModalOpen(Modals.options);
    }

    void ExitClick(GameObject go) {
        //return to main
        Object.Destroy(MusicManager.instance.gameObject);
        Main.instance.sceneManager.LoadScene(Main.instance.startScene);
    }
}
