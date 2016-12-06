using UnityEngine;

public class Tile {
  public int TileType;
  public GameObject GO;
  public TileControl TileControl;

  public Tile () {
    TileType = -1;
  }

  public Tile (int tileType, GameObject go, TileControl tileControl) {
    TileType = tileType;
    GO = go;
    TileControl = tileControl;
  }
  public Tile cloneTile () {
    return new Tile(TileType, GO, TileControl);
  }
}