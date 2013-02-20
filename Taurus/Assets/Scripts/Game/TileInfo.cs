using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TileInfo : MonoBehaviour {
    public int layerIndexInfo = 1;

    private static TileInfo mInstance;

    private tk2dTileMap mMap;

    public tk2dTileMap map {
        get {
            if(mMap == null)
                SetTileMap();

            return mMap; 
        } 
    }

    public static TileInfo instance { 
        get {
            if(mInstance == null) {
                mInstance = (TileInfo)Object.FindObjectOfType(typeof(TileInfo));
            }

            return mInstance; 
        } 
    }

    void OnDestroy() {
        mInstance = null;
    }

    void Awake() {
        mInstance = this;

        SetTileMap();
    }

    private void SetTileMap() {
        GameObject mapObj = GameObject.FindGameObjectWithTag(Layers.tagMap);

        if(mapObj != null)
            mMap = mapObj.GetComponent<tk2dTileMap>();
    }
}
