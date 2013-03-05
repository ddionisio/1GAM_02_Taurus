using UnityEngine;
using System.Collections;

public class Firey : MonoBehaviour {
    public const float refreshDelay = 0.01f;

    public Transform template; //this should match the dir

    public Transform endCap;

    public Transform container; //place to put the generated objects

    public Dir dir;

    public LayerMask solidCheck;

    private int mLineCount = 0;

    void OnDestroy() {
        if(ActionManager.instance != null) {
            ActionManager.instance.actAddCallback -= OnActAdded;
            ActionManager.instance.actUndoCallback -= OnActUndone;
        }
    }

    // Use this for initialization
    void Start() {
        if(container == null)
            container = transform;

        TileAlign ta = GetComponent<TileAlign>();
        ta.Align();

        ActionManager.instance.actAddCallback += OnActAdded;
        ActionManager.instance.actUndoCallback += OnActUndone;

        //determine line objects
        tk2dTileMap map = TileInfo.instance.map;

        switch(dir) {
            case Dir.North:
                for(int r = ta.row+1; r < map.height; r++) {
                    int id = map.Layers[TileInfo.instance.layerIndexInfo].GetTile(ta.col, r);
                    tk2dRuntime.TileMap.TileInfo inf = map.GetTileInfoForTileId(id);

                    if(inf != null && (TileType)inf.intVal == TileType.Wall) {
                        break;
                    }
                    else {
                        Transform newObj = (Transform)Object.Instantiate(template);
                        newObj.parent = container;
                    }
                }
                break;

            case Dir.South:
                for(int r = ta.row-1; r >= 0; r--) {
                    int id = map.Layers[TileInfo.instance.layerIndexInfo].GetTile(ta.col, r);
                    tk2dRuntime.TileMap.TileInfo inf = map.GetTileInfoForTileId(id);

                    if(inf != null && (TileType)inf.intVal == TileType.Wall) {
                        break;
                    }
                    else {
                        Transform newObj = (Transform)Object.Instantiate(template);
                        newObj.parent = container;
                    }
                }
                break;

            case Dir.West:
                for(int c = ta.col-1; c >= 0; c--) {
                    int id = map.Layers[TileInfo.instance.layerIndexInfo].GetTile(c, ta.row);
                    tk2dRuntime.TileMap.TileInfo inf = map.GetTileInfoForTileId(id);

                    if(inf != null && (TileType)inf.intVal == TileType.Wall) {
                        break;
                    }
                    else {
                        Transform newObj = (Transform)Object.Instantiate(template);
                        newObj.parent = container;
                    }
                }
                break;

            case Dir.East:
                for(int c = ta.col+1; c < map.width; c++) {
                    int id = map.Layers[TileInfo.instance.layerIndexInfo].GetTile(c, ta.row);
                    tk2dRuntime.TileMap.TileInfo inf = map.GetTileInfoForTileId(id);

                    if(inf != null && (TileType)inf.intVal == TileType.Wall) {
                        break;
                    }
                    else {
                        Transform newObj = (Transform)Object.Instantiate(template);
                        newObj.parent = container;
                    }
                }
                break;
        }

        StartCoroutine(RefreshLineDelay());
    }

    void OnActUndone(Actor actor, Act act, Dir dir, object dat) {
        if(act == Act.Teleport)
            StartCoroutine(RefreshLineDelay());
        else if(act == Act.Move) {
            Enemy enemy = actor as Enemy;
            if(enemy != null && enemy.fireImmune) {
                StartCoroutine(RefreshLineDelay());
            }
        }
    }

    void OnActAdded(Actor actor, Act act, Dir dir, object dat) {
        if(act == Act.Teleport)
            StartCoroutine(RefreshLineDelay());
        else if(act == Act.MoveEnd) {
            Enemy enemy = actor as Enemy;
            if(enemy != null) {
                if(enemy.fireImmune) {
                    StartCoroutine(RefreshLineDelay());
                }
                else if(!enemy.dead && CheckCollision(actor))
                    enemy.Die(dir);
            }
            else {
                Player player = actor as Player;
                if(player != null) {
                    if(!player.dead && CheckCollision(actor))
                        PlayerController.KillPlayer(player);
                }
            }
        }
    }

    IEnumerator RefreshLineDelay() {
        yield return new WaitForFixedUpdate();

        RefreshLine();

        //check if we hit a player
        if(PlayerController.players != null) {
            foreach(Player player in PlayerController.players) {
                if(player != null && !player.dead && CheckCollision(player)) {
                    PlayerController.KillPlayer(player);
                    break;
                }
            }
        }

        yield break;
    }

    private bool CheckCollision(Actor actor) {
        bool ret = false;

        TileAlign ta = GetComponent<TileAlign>();

        TileAlign actorTa = actor.tile;

        switch(dir) {
            case Dir.North:
                for(int r = ta.row, i = 0; i < mLineCount; i++, r++) {
                    if(actorTa.row == r && actorTa.col == ta.col) {
                        ret = true;
                        break;
                    }
                }
                break;

            case Dir.South:
                for(int r = ta.row - 1, i = 0; i < mLineCount; i++, r--) {
                    if(actorTa.row == r && actorTa.col == ta.col) {
                        ret = true;
                        break;
                    }
                }
                break;

            case Dir.West:
                for(int c = ta.col - 1, i = 0; i < mLineCount; i++, c--) {
                    if(actorTa.row == ta.row && actorTa.col == c) {
                        ret = true;
                        break;
                    }
                }
                break;

            case Dir.East:
                for(int c = ta.col + 1, i = 0; i < mLineCount; i++, c++) {
                    if(actorTa.row == ta.row && actorTa.col == c) {
                        ret = true;
                        break;
                    }
                }
                break;
        }
        return ret;
    }

    //true if line pos applied, false if found solid
    private bool ApplyLinePos(TileAlign ta, int childInd, int row, int col) {
        tk2dTileMap map = TileInfo.instance.map;

        int id = map.Layers[TileInfo.instance.layerIndexInfo].GetTile(col, row);
        tk2dRuntime.TileMap.TileInfo inf = map.GetTileInfoForTileId(id);

        if(inf != null && (TileType)inf.intVal == TileType.Wall) {
            return false;
        }

        //check if there's something in the way
        Vector3 pos = map.GetTilePosition(col, row);
        pos.x += ta.offset.x;
        pos.y += ta.offset.y;
        pos.z = transform.position.z;

        if(Physics.Raycast(new Vector3(pos.x, pos.y, -1000), Vector3.forward, Mathf.Infinity, solidCheck.value)) {
            return false;
        }

        Transform line = container.GetChild(childInd);
        line.gameObject.SetActive(true);
        line.position = pos;

        return true;
    }

    private void RefreshLine() {
        TileAlign ta = GetComponent<TileAlign>();

        mLineCount = 0;

        //determine line objects
        int childInd = 0;

        tk2dTileMap map = TileInfo.instance.map;

        switch(dir) {
            case Dir.North:
                for(int r = ta.row+1; r < map.height; r++) {
                    if(ApplyLinePos(ta, childInd, r, ta.col)) {
                        mLineCount++;
                        childInd++;
                    }
                    else
                        break;
                }
                break;

            case Dir.South:
                for(int r = ta.row-1; r >= 0; r--) {
                    if(ApplyLinePos(ta, childInd, r, ta.col)) {
                        mLineCount++;
                        childInd++;
                    }
                    else
                        break;
                }
                break;

            case Dir.West:
                for(int c = ta.col-1; c >= 0; c--) {
                    if(ApplyLinePos(ta, childInd, ta.row, c)) {
                        mLineCount++;
                        childInd++;
                    }
                    else
                        break;
                }
                break;

            case Dir.East:
                for(int c = ta.col + 1; c < map.width; c++) {
                    if(ApplyLinePos(ta, childInd, ta.row, c)) {
                        mLineCount++;
                        childInd++;
                    }
                    else
                        break;
                }
                break;
        }

        //set end cap
        Vector3 endCapPos;

        if(childInd > 0) {
            int endInd = childInd - 1;

            endCapPos = container.GetChild(endInd).position;
        }
        else {
            endCapPos = transform.position;
        }

        endCap.position = new Vector3(endCapPos.x, endCapPos.y, endCap.position.z);

        //deactivate the rest of lines
        for(int max = container.GetChildCount(); childInd < max; childInd++) {
            container.GetChild(childInd).gameObject.SetActive(false);
        }
    }
}
