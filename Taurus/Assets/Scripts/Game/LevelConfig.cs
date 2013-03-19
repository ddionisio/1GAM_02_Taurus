using UnityEngine;
using System.Collections.Generic;

//level configuration
public class LevelConfig : MonoBehaviour {
    //use for localize text
    public const string levelTitlePrefix = "leveltitle";
    public const string levelVictoryPrefix = "levelwin";

    //use for slot data
    public const string levelCountKey = "lvlcount";
    public const string secretCountKey = "slvlcount";

    public const string levelPrefix = "lvl";
    public const string secretPrefix = "slvl";

    public class Info {
        public string cutscene; //cutscene
    }

    public TextAsset config;

    public string endingScene;
    public string secretScene;

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

    public bool CheckLevelSecretUnlock(int level) {
        int sub = level / 31;

        int mask = 1 << (level % 31);

        int flags = UserData.instance.GetInt(secretPrefix + sub, 0);

        return (flags & mask) != 0;
    }

    public void SaveLevelUnlock(int level, bool secretUnlocked) {
        int sub = level / 31;

        int mask = 1 << (level % 31);

        string key = levelPrefix + sub;

        int flags = UserData.instance.GetInt(key, 0);
        UserData.instance.SetInt(key, flags | mask);

        if(secretUnlocked) {
            string _key = secretPrefix + sub;
            int _flags = UserData.instance.GetInt(_key, 0);
            UserData.instance.SetInt(_key, _flags | mask);
        }
                
        UserData.instance.Save();

        SaveLevelUnlockCount();
                
        PlayerPrefs.Save();
    }

    public void Ending() {
        if(!string.IsNullOrEmpty(endingScene)) {
            //ending
            UserSlotData usd = (UserSlotData)UserData.instance;
            int numLvlComplete = UserSlotData.GetSlotValueInt(usd.curSlot, levelCountKey, 0);
            int numSecrets = UserSlotData.GetSlotValueInt(usd.curSlot, secretCountKey, 0);

            if(numLvlComplete == numSecrets && !string.IsNullOrEmpty(secretScene)) {
                Main.instance.sceneManager.LoadScene(secretScene);
            }
            else {
                Main.instance.sceneManager.LoadScene(endingScene);
            }
        }
        else {
            //go back to level select
            Main.instance.sceneManager.LoadScene(Scenes.levelSelect);
        }
    }

    //returns true if ending
    public bool LoadLevel(int level) {
        if(level < mLevels.Count) {
            mLevelToLoad = level;

            if(!string.IsNullOrEmpty(mLevels[level].cutscene)) {
                Main.instance.sceneManager.LoadScene(mLevels[level].cutscene);
            }
            else {
                Main.instance.sceneManager.LoadLevel(mLevelToLoad);
            }

            return false;
        }
        else {
            Ending();

            return true;
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

    private void SaveLevelUnlockCount() {
        int unlockCount = 0;
        int secretCount = 0;

        int curSub = -1;
        int flags = 0;
        int sflags = 0;

        for(int lvl = 0; lvl < mLevels.Count; lvl++) {
            int sub = lvl / 31;
            if(curSub != sub) {
                curSub = sub;

                string key = levelPrefix + sub;
                flags = UserData.instance.GetInt(key, 0);

                string _key = secretPrefix + sub;
                sflags = UserData.instance.GetInt(_key, 0);
            }

            int mask = 1 << (lvl % 31);

            if((flags & mask) != 0)
                unlockCount++;

            if((sflags & mask) != 0)
                secretCount++;
        }

        UserSlotData usd = (UserSlotData)UserData.instance;
        UserSlotData.SetSlotValueInt(usd.curSlot, levelCountKey, unlockCount);
        UserSlotData.SetSlotValueInt(usd.curSlot, secretCountKey, secretCount);
    }
}
