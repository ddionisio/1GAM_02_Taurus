using UnityEngine;
using System.Collections;

public class ModalVictory : UIController {
    public const string bablePrefix = "bable";
    public const int bableMax = 8;

    public GameObject clickArea;

    public UILabel bable;

    protected override void OnActive(bool active) {
        InputManager input = Main.instance.input;

        if(active) {
            input.AddButtonCall(InputAction.MenuEscape, OnInputEsc);
            input.AddButtonCall(InputAction.MenuEnter, OnInputProceed);

            if(clickArea != null)
                UIEventListener.Get(clickArea).onClick = OnClickArea;
        }
        else {
            input.RemoveButtonCall(InputAction.MenuEscape, OnInputEsc);
            input.RemoveButtonCall(InputAction.MenuEnter, OnInputProceed);

            if(clickArea != null)
                UIEventListener.Get(clickArea).onClick = null;
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
        if(data.state == InputManager.State.Pressed) {
            GoNextLevel();
        }
    }

    void OnClickArea(GameObject go) {
        GoNextLevel();
    }

    private void GoNextLevel() {
        if(LevelConfig.instance.LoadLevel(Main.instance.sceneManager.curLevel + 1)) {
            //remove music manager, ending has its own
            Object.Destroy(MusicManager.instance.gameObject);
        }
    }
}