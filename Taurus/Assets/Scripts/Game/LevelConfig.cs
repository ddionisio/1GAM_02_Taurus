﻿using UnityEngine;
using System.Collections.Generic;

//level configuration
public class LevelConfig : MonoBehaviour {
    public const string levelCountKey = "lvlcount";
    public const string levelPrefix = "lvl";

    public class Info {
        public string cutscene; //cutscene
    }

    public TextAsset config;

    public string endingScene;

    private static LevelConfig mInstance = null;

    private List<Info> mLevels;

    private int mLevelToLoad = -1;

    public static LevelConfig instance { get { return mInstance; } }

    public int numLevels { get { return mLevels != null ? mLevels.Count : 0; } }

    public Info GetInfo(int level) {
        return mLevels[level];
    }

    public bool CheckLevelUnlock(int level) {
        int sub = level / 31;

        int mask = 1 << (level % 31);

        int flags = UserData.instance.GetInt(levelPrefix + sub, 0);

        return (flags & mask) != 0;
    }

    public void SaveLevelUnlock(int level) {
        int sub = level / 31;

        int mask = 1 << (level % 31);

        string key = levelPrefix + sub;

        int flags = UserData.instance.GetInt(key, 0);
        UserData.instance.SetInt(key, flags | mask);
        UserData.instance.Save();

        PlayerPrefs.Save();
    }

    public void LoadLevel(int level) {
        if(level < mLevels.Count) {
            mLevelToLoad = level;

            if(!string.IsNullOrEmpty(mLevels[level].cutscene)) {
                Main.instance.sceneManager.LoadScene(mLevels[level].cutscene);
            }
            else {
                Main.instance.sceneManager.LoadLevel(mLevelToLoad);
            }
        }
        else if(!string.IsNullOrEmpty(endingScene)) {
            //ending
            Main.instance.sceneManager.LoadScene(endingScene);
        }
        else {
            //go back to level select
            Main.instance.sceneManager.LoadScene(Scenes.levelSelect);
        }
    }

    /// <summary>
    /// This is called in the cutscene after it finishes
    /// </summary>
    public void LoadLevelProceed() {
        if(mLevelToLoad != -1) {
            Main.instance.sceneManager.LoadLevel(mLevelToLoad);
        }
    }

    void OnDestroy() {
        mInstance = null;
    }

    void Awake() {
        mInstance = this;

        //load file
        if(config != null) {
            fastJSON.JSON.Instance.Parameters.UseExtensions = false;
            mLevels = fastJSON.JSON.Instance.ToObject<List<Info>>(config.text);
        }
    }
}