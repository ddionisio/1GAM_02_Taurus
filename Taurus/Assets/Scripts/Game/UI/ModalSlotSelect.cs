using UnityEngine;
using System.Collections;

public class ModalSlotSelect : UIController {

    public UIEventListener[] slots;

    private UISlot mSlotSelected;

    protected override void OnActive(bool active) {
        if(active) {
            foreach(UIEventListener slot in slots) {
                slot.onClick = SelectSlotClick;
            }

            Main.instance.input.AddButtonCall(InputAction.MenuDelete, OnDeleteButton);
        }
        else {
            foreach(UIEventListener slot in slots) {
                slot.onClick = null;
            }

            Main.instance.input.RemoveButtonCall(InputAction.MenuDelete, OnDeleteButton);
        }
    }

    protected override void OnOpen() {
    }

    protected override void OnClose() {
    }

    void SelectSlotClick(GameObject go) {
        UISlot uiSlot = go.GetComponentInChildren<UISlot>();
        if(uiSlot != null) {
            UserSlotData usd = (UserSlotData)UserData.instance;

            int slot = uiSlot.slot;

            if(UserSlotData.IsSlotAvailable(slot)) {
                //load slot
                usd.SetSlot(slot, true);

                //level select
                Main.instance.sceneManager.LoadScene(Scenes.levelSelect);
            }
            else {
                //start new game, first change the name
                UIModalManager.UIData uid = UIModalManager.instance.ModalGetData(Modals.newGame);
                ModalNewGame modalNewGame = (ModalNewGame)uid.ui;
                modalNewGame.slot = slot;

                UIModalManager.instance.ModalOpen(Modals.newGame);
            }
        }
    }

    void OnDeleteButton(InputManager.Info data) {
        if(UICamera.selectedObject != null) {
            mSlotSelected = UICamera.selectedObject.GetComponentInChildren<UISlot>();
            if(mSlotSelected != null) {
                int slot = mSlotSelected.slot;

                if(UserSlotData.IsSlotAvailable(slot)) {
                    string title = GameLocalize.GetText("eraseSlotTitle");
                    title = string.Format(title, mSlotSelected.nameLabel.text);

                    UIModalConfirm.Open(title, null, OnConfirm);
                }
            }
        }
    }

    void OnConfirm(bool yes) {
        if(yes) {
            UserSlotData.DeleteSlot(mSlotSelected.slot);
            UserSlotData.SetSlotValueInt(mSlotSelected.slot, LevelConfig.levelCountKey, 0);
            UserSlotData.SetSlotValueInt(mSlotSelected.slot, LevelConfig.secretCountKey, 0);

            mSlotSelected.RefreshLabels();
        }
    }
}
