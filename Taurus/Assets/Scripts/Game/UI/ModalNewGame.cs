using UnityEngine;
using System.Collections;

//TODO: portability
public class ModalNewGame : UIController {
    public UIInput input;

    public NGUIPage intro;

    [System.NonSerialized]
    public int slot;

    protected override void OnActive(bool active) {

        UICamera uiCam = UICamera.eventHandler;

        if(active) {
            uiCam.useKeyboard = true;
            uiCam.submitKey0 = KeyCode.Return;
            uiCam.submitKey1 = KeyCode.KeypadEnter;

            UICamera.selectedObject = input.gameObject;
            input.text = "";
        }
        else {
            uiCam.useKeyboard = false;
            uiCam.submitKey0 = KeyCode.None;
            uiCam.submitKey1 = KeyCode.None;
        }
    }

    protected override void OnOpen() {
    }

    protected override void OnClose() {
    }

    void OnSubmit(string inputString) {
        if(!string.IsNullOrEmpty(inputString)) {
            Debug.Log("creating slot: " + slot + " name: " + inputString);

            //set the slot, set name, and save
            UserSlotData usd = (UserSlotData)UserData.instance;
            usd.SetSlot(slot, true);
            usd.slotName = inputString;

            usd.Save();
            PlayerPrefs.Save();

            //play cheesy intro
            if(intro != null) {
                UIModalManager.instance.ModalCloseAll();
                intro.gameObject.SetActive(true);
            }
            else
                Main.instance.sceneManager.LoadScene(Scenes.levelSelect);
        }
        else {
            UIModalManager.instance.ModalCloseTop();
            //error dialog?
        }
    }

    void OnIntroPageEnd() {
        //once intro is done, enter the level select
        Main.instance.sceneManager.LoadScene(Scenes.levelSelect);
    }

    void Awake() {
        if(intro != null) {
            intro.gameObject.SetActive(false);
            intro.pageEndCallback = OnIntroPageEnd;
        }
    }
}