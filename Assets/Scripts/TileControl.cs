using UnityEngine;
using System.Collections;

public class TileControl : MonoBehaviour {
  public GridManager GridManager;
  public Position position;

  public void Move (Position xy) {
    StartCoroutine(Moving(xy));
  }

  IEnumerator Moving (Position xy) {
    GridManager.ReportTileMovement();

    Vector2 destination = new Vector2(xy.X, xy.Y);

    bool moving = true;

    while (moving) {
      transform.position = Vector2.MoveTowards(transform.position, destination, 4f * Time.deltaTime);
      if (Vector2.Distance(transform.position, destination) <= 0.1f) {
        transform.position = destination;
        moving = false;
      }
      yield return null;
    }
    position = xy;
    gameObject.name = xy.X + "/" + xy.Y;
    GridManager.ReportTileStopped();
  }
}