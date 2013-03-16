using UnityEngine;
using System.Collections;

public class SoundPlayerOnSelect : SoundPlayer {

    void OnSelect(bool yes) {
        if(yes)
            Play();
    }
}
