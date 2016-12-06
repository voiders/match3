using UnityEngine;

public class PlayerInput : MonoBehaviour {
  private GridManager gridManager;
  public LayerMask Tiles;
  private GameObject activeTile;

  void Awake() {
    gridManager = GetComponent<GridManager>();
  }
  void Update() {
    if (Input.GetKeyDown(KeyCode.Mouse0)) {
      if (activeTile == null) {
        SelectTile();
      } else {
        AttempMove();
      }
    } else if (Input.GetKeyDown(KeyCode.Mouse1)) {
      activeTile = null;
    }
  }
  
  void SelectTile () {
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 50f, Tiles);
    if (hit) {
      activeTile = hit.collider.gameObject;
    }
  }
  void AttempMove () {
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 50f, Tiles);
    if (hit) {
      if (Vector2.Distance(activeTile.transform.position, hit.collider.gameObject.transform.position) <= 1.25f){
        TileControl activeControl = activeTile.GetComponent<TileControl>();
        TileControl hitControl = hit.collider.gameObject.GetComponent<TileControl>();

        Position activeXY = activeControl.position;
        Position hitXY = hitControl.position;

        activeControl.Move(hitXY);
        hitControl.Move(activeXY);

        gridManager.SwitchTiles(hitXY, activeXY);

        activeTile = null;
      }
    }
  }
}