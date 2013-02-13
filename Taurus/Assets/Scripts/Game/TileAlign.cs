using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TileAlign : MonoBehaviour {
    public Vector2 offset;
    public bool doAlign = false;

    private int mRow = -1;
    private int mCol = -1;

    public int row { get { return mRow; } }
    public int col { get { return mCol; } }

    public tk2dRuntime.TileMap.TileInfo tileData {
        get {
            tk2dTileMap map = TileInfo.instance.map;

            if(mRow == -1 || mCol == -1)
                Align();

            int id = map.Layers[TileInfo.instance.layerIndexInfo].GetTile(mCol, mRow);

            return map.GetTileInfoForTileId(id);
        }
    }

    public tk2dRuntime.TileMap.TileInfo GetTileInfo(Dir toDir) {
        if(mRow == -1 || mCol == -1)
            Align();

        int c = mCol, r = mRow;

        switch(toDir) {
            case Dir.North:
                r++;
                break;

            case Dir.South:
                r--;
                break;

            case Dir.West:
                c--;
                break;

            case Dir.East:
                c++;
                break;
        }

        tk2dTileMap map = TileInfo.instance.map;

        int id = c >= 0 && r >= 0 && c < map.partitionSizeX && r < map.partitionSizeY ? map.Layers[TileInfo.instance.layerIndexInfo].GetTile(c, r) : -1;

        return map.GetTileInfoForTileId(id);
    }
    
    public void Set(int col, int row) {
        mCol = col; mRow = row;
        transform.position = GetTilePos(mCol, mRow);
    }

    public Vector3 GetTilePos(int col, int row) {
        Vector3 alignPos = TileInfo.instance.map.GetTilePosition(col, row);
        return new Vector3(alignPos.x + offset.x, alignPos.y + offset.y, transform.position.z);
    }

    public void Align() {
        if(TileInfo.instance != null) {
            tk2dTileMap map = TileInfo.instance.map;
            if(map != null) {
                map.GetTileAtPosition(transform.position, out mCol, out mRow);
                transform.position = GetTilePos(mCol, mRow);
            }
        }
    }

    // Use this for initialization
    void Start() {
        //align
        Align();
    }

#if UNITY_EDITOR
    // Update is called once per frame
    void Update() {
        if(doAlign) {
            Align();
            doAlign = false;
        }
    }
#endif

    void OnDrawGizmosSelected() {
        Vector3 pos = transform.position;
        pos.x += offset.x;
        pos.y += offset.y;

        Gizmos.DrawIcon(pos, "cross");
    }
}
