using UnityEngine;
using System.Collections;

public class SoundPlayerGlobalOnClick : MonoBehaviour {
    public string id;

    void OnClick() {
        SoundPlayerGlobal.instance.Play(id);
    }
}
