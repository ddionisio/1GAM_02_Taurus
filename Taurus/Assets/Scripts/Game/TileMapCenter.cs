using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TileMapCenter : MonoBehaviour {
    public Vector2 ofs;
    public bool updateCenter = false;

    private tk2dTileMap mMap;

    void Awake() {
        GameObject mapObj = GameObject.FindGameObjectWithTag(Layers.tagMap);

        if(mapObj != null)
            mMap = mapObj.GetComponent<tk2dTileMap>();
    }

    void Start() {
        DoCenter();
    }

#if UNITY_EDITOR
    // Update is called once per frame
    void Update() {
        if(updateCenter) {
            DoCenter();
            updateCenter = false;
        }
    }
#endif

    private void DoCenter() {
        Vector2 tileOfs = mMap.transform.position;
        tileOfs.x += mMap.data.tileOrigin.x;
        tileOfs.y += mMap.data.tileOrigin.y;

        Vector2 mapSize = new Vector2((float)(mMap.width * mMap.partitionSizeX), (float)(mMap.height * mMap.partitionSizeY));

        Vector2 center = tileOfs + (mapSize * 0.5f);

        transform.position = new Vector3(center.x + ofs.x, center.y + ofs.y, transform.position.z);
    }
}
