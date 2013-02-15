using UnityEngine;
using System.Collections;

public class ModalPause : UIController {
    public UIEventListener restart;
    public UIEventListener options;
    public UIEventListener exit;

    protected override void OnActive(bool active) {
        if(active) {
            restart.onClick = RestartClick;
            options.onClick = OptionsClick;
            exit.onClick = ExitClick;
        }
        else {
            restart.onClick = null;
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
        Debug.Log("options?");
    }

    void ExitClick(GameObject go) {
        //return to level select
        Main.instance.sceneManager.LoadScene(Scenes.levelSelect);
    }
}
