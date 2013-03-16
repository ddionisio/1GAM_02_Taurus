using UnityEngine;
using System.Collections;

public class SoundPlayerGlobalOnValueChange : MonoBehaviour {
    public string id;

    void OnValueChange(float val) {
        SoundPlayerGlobal.instance.Play(id);
    }
}
