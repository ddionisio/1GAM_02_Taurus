using UnityEngine;
using System.Collections;

public class ModalPause : UIController {
    public UIEventListener restart;
    public UIEventListener help;
    public UIEventListener options;
    public UIEventListener exit;

    protected override void OnActive(bool active) {
        if(active) {
            restart.onClick = RestartClick;
            help.onClick = HelpClick;
            options.onClick = OptionsClick;
            exit.onClick = ExitClick;
        }
        else {
            restart.onClick = null;
            help.onClick = null;
            options.onClick = null;
            exit.onClick = null;
        }
    }

    protected override void OnOpen() {
    }

    protected override void OnClose() {
    }

    void RestartClick(GameObject go) {
        Main.instance.sceneManager.Reload();
    }

    void OptionsClick(GameObject go) {
        UIModalManager.instance.ModalOpen(Modals.options);
    }

    void HelpClick(GameObject go) {
        UIModalManager.instance.ModalOpen(Modals.help);
    }

    void ExitClick(GameObject go) {
        //return to level select
        Main.instance.sceneManager.LoadScene(Scenes.levelSelect);
    }
}
