using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour {

  public GameObject[] TilePrefabs;

  public int score = 0;
  public int GridWidth;
  public int GridHeight;
  public Tile[,] Grid;

  private int movingTiles;

  void Awake() {
    CreateGrid();
    // transform.position = new Vector2(
    //   transform.position.x - (GridWidth / 2),
    //   transform.position.y - (GridHeight / 2)
    // );
  }

  void CreateGrid() {
    Grid = new Tile[GridWidth, GridHeight];
    for (int x = 0; x < GridWidth; x++) {
      for (int y = 0; y < GridHeight; y++) {
        int randomTileType = Random.Range(0, TilePrefabs.Length);
        Grid[x, y] = NewTile(randomTileType, x, y);
      }
    }
  }

  public Tile NewTile (int type, int x, int y) {
    GameObject go = Instantiate(TilePrefabs[type], new Vector2(x, y), Quaternion.identity) as GameObject;
    go.transform.parent = gameObject.transform;
    TileControl tileControl = go.GetComponent<TileControl>();
    tileControl.GridManager = this;
    tileControl.position = new Position(x, y);
    go.name = x + "/" + y;
    return new Tile(type, go, tileControl);
  }

  public void SwitchTiles (Position firstXY, Position secondXY) {
    Tile tempTile = Grid[secondXY.X, secondXY.Y].cloneTile();
    Grid[secondXY.X, secondXY.Y] = Grid[firstXY.X, firstXY.Y].cloneTile();
    Grid[firstXY.X, firstXY.Y] = tempTile;
  }

  public void CheckMatches () {
    List<Position> checkingTiles = new List<Position>();
    List<Position> tilesToDestroy = new List<Position>();

    for (int x = 0; x < GridWidth; x++) {
      int currentTileType = -1;
      int lastTileType = -1;

      if (checkingTiles.Count >= 3) {
        tilesToDestroy.AddRange(checkingTiles);
      }

      checkingTiles.Clear();

      for (int y = 0; y < GridHeight; y++) {
        currentTileType = Grid[x, y].TileType;

        if (currentTileType != lastTileType) {
          if (checkingTiles.Count >= 3) {
            tilesToDestroy.AddRange(checkingTiles);
          }
          checkingTiles.Clear();
        }
        checkingTiles.Add(new Position(x, y));
        lastTileType = currentTileType;
      }
    }
    checkingTiles.Clear();

    for (int y = 0; y < GridHeight; y++) {
      int currentTileType = -1;
      int lastTileType = -1;

      if (checkingTiles.Count >= 3) {
        for (int i = 0; i < checkingTiles.Count; i++) {
          if (!tilesToDestroy.Contains(checkingTiles[i])) {
            tilesToDestroy.Add(checkingTiles[i]);
          }
        }
      }

      checkingTiles.Clear();

      for (int x = 0; x < GridWidth; x++) {
        currentTileType = Grid[x, y].TileType;
        if (currentTileType != lastTileType) {
          if (checkingTiles.Count >= 3) {
            for (int i = 0; i < checkingTiles.Count; i++) {
              if (!tilesToDestroy.Contains(checkingTiles[i])) {
                tilesToDestroy.Add(checkingTiles[i]);
              }
            }
          }
          checkingTiles.Clear();
        }
        checkingTiles.Add(new Position(x, y));
        lastTileType = currentTileType;
      }
    }

    if (tilesToDestroy.Count != 0) {
      DestroyMatches(tilesToDestroy);
    } else {
      ReplaceTiles();
    }
  }

  void DestroyMatches (List<Position> tilesToDestroy) {
    for (int i = 0; i < tilesToDestroy.Count; i++) {
      Destroy(Grid[tilesToDestroy[i].X, tilesToDestroy[i].Y].GO);
      Grid[tilesToDestroy[i].X, tilesToDestroy[i].Y] = new Tile();
      if (i <= 2) {
        AddScore(10);
      } else {
        AddScore(20);
      }
    }
    GravityCheck();
  }
  void ReplaceTiles () {
    Debug.Log("ReplaceTiles");
    for (int x = 0; x < GridWidth; x++) {
      int missingTileCount = 0;
      for (int y = 0; y < GridHeight; y++) {
        if (Grid[x, y].TileType == -1) {
          missingTileCount++;
        }
      }
      for (int i = 0; i < missingTileCount; i++) {
        int tileY = GridHeight - missingTileCount + i;
        int randomTileType = Random.Range(0, TilePrefabs.Length);
        GameObject go = Instantiate(TilePrefabs[randomTileType], new Vector2(x, GridHeight + i), Quaternion.identity) as GameObject;
        TileControl tileControl = go.GetComponent<TileControl>();
        tileControl.GridManager = this;
        tileControl.Move(new Position(x, tileY));
        Grid[x, tileY] = new Tile(randomTileType, go, tileControl);
        go.name = x + "/" + tileY;
      }
    }
  }
  void GravityCheck() {
    for (int x = 0; x < GridWidth; x++) {
      int missingTileCount = 0;
      for (int y = 0; y < GridHeight; y++) {
        if (Grid[x, y].TileType == -1){
          missingTileCount++;
        } else {
          if (missingTileCount >= 1) {
            Tile tile = new Tile(Grid[x, y].TileType, Grid[x, y].GO, Grid[x, y].TileControl);
            Grid[x, y].TileControl.Move(new Position(x, y - missingTileCount));
            Grid[x, y - missingTileCount] = tile;
            Grid[x, y] = new Tile();
          }
        }
      }
    }
    ReplaceTiles();
  }

  public void ReportTileMovement () {
    movingTiles++;
  }

  // If tiles have been moving, we'll check for matches once they are all done.
  public void ReportTileStopped () {
    movingTiles--;

    if (movingTiles == 0) {
      CheckMatches();
    }
  }

  void AddScore (int amount) {
    score += amount;
  }
}
