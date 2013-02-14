using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIModalManager : MonoBehaviour {
    [System.Serializable]
    public class UIData {
        public string name;
        public UIController ui;
        public bool exclusive = true; //hide modals behind
    }

    public UIData[] uis;

    public string openOnStart = "";

    public static UIModalManager instance {
        get {
            return mInstance;
        }
    }

    private static UIModalManager mInstance;

    private Dictionary<string, UIData> mModals;
    private Stack<UIData> mModalStack;

    public UIData ModalGetData(string modal) {
        UIData dat = null;
        mModals.TryGetValue(modal, out dat);
        return dat;
    }

    public bool ModalIsInStack(string modal) {
        bool ret = false;
        foreach(UIData uid in mModalStack) {
            if(uid.name == modal) {
                ret = true;
                break;
            }
        }
        return ret;
    }

    public string ModalGetTop() {
        string ret = "";
        if(mModalStack.Count > 0) {
            ret = mModalStack.Peek().name;
        }
        return ret;
    }

    //closes all modal and open this
    public void ModalReplace(string modal) {
        ModalClearStack(false);
        ModalPushToStack(modal, false);

    }

    public void ModalOpen(string modal) {
        ModalPushToStack(modal, true);
    }

    public void ModalCloseTop() {
        if(mModalStack.Count > 0) {
            UIData uid = mModalStack.Pop();
            UIController ui = uid.ui;
            ui._active(false);
            ui._close();
            ui.gameObject.SetActive(false);

            if(mModalStack.Count == 0) {
                ModalInactive();
            }
            else {
                //re-show top
                UIData prevUID = mModalStack.Peek();
                UIController prevUI = prevUID.ui;
                if(!prevUI.gameObject.activeSelf) {
                    prevUI.gameObject.SetActive(true);
                }

                prevUI._active(true);
            }
        }
    }

    public void ModalCloseAll() {
        ModalClearStack(true);
    }

    void ModalPushToStack(string modal, bool evokeActive) {
        if(evokeActive && mModalStack.Count == 0) {
            SceneManager.RootBroadcastMessage("OnUIModalActive", null, SendMessageOptions.DontRequireReceiver);
        }

        UIData uid = mModals[modal];

        if(mModalStack.Count > 0) {
            //hide below
            UIData prevUID = mModalStack.Peek();
            UIController prevUI = prevUID.ui;
            prevUI._active(false);

            if(uid.exclusive)
                prevUI.gameObject.SetActive(false);
        }

        UIController ui = uid.ui;
        ui.gameObject.SetActive(true);
        ui._open();
        ui._active(true);

        mModalStack.Push(uid);
    }

    void ModalClearStack(bool evokeInactive) {
        if(mModalStack.Count > 0) {
            foreach(UIData uid in mModalStack) {
                UIController ui = uid.ui;
                ui._active(false);
                ui._close();
                ui.gameObject.SetActive(false);
            }

            mModalStack.Clear();

            if(evokeInactive) {
                ModalInactive();
            }
        }
    }

    void ModalInactive() {
        SceneManager.RootBroadcastMessage("OnUIModalInactive", null, SendMessageOptions.DontRequireReceiver);
    }

    void SceneChange() {
        ModalCloseAll();
    }

    void OnDestroy() {
        mInstance = null;
    }

    void Awake() {
        mInstance = this;

        mModals = new Dictionary<string, UIData>(uis.Length);
        mModalStack = new Stack<UIData>(uis.Length);

        //setup data and deactivate object
        for(int i = 0; i < uis.Length; i++) {
            UIData uid = uis[i];
            UIController ui = uid.ui;
            if(ui != null) {
                ui.gameObject.SetActive(false);
            }

            mModals.Add(uid.name, uid);
        }
    }

    void Start() {
        if(!string.IsNullOrEmpty(openOnStart)) {
            ModalOpen(openOnStart);
        }
    }
}
