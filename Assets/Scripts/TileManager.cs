using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    //public NormalTile normalTile;
    //public Player selectedUnit;

    public int gridSize;

    public enum TileId
    {
        EMPTY = 0,
        ROAD = 1,
        HOUSE = 2
    }

    [SerializeField]
    public GameObject roadPrefab;

    public static TileManager Instance;

    TileId[,] tile = new TileId[100, 100];

    private void OnMouseUp()
    {
        // left click - get info from selected tile
        if (Input.GetMouseButtonUp(0))
        {
            // save the camera as public field if you using not the main camera
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // get the collision point of the ray with the z = 0 plane
            if (Physics.Raycast(ray, out RaycastHit hitPoint, 1000f))
            {
                Vector3 gridLocation = ToGridPos(hitPoint.point);

                int x = Mathf.RoundToInt(gridLocation.x) / gridSize, y = Mathf.RoundToInt(gridLocation.z) / gridSize;
                Debug.Log("ROAD x " + x + " y " + y);
                if (CanBePlaced(x, y, TileId.ROAD))
                {
                    SetTile(x, y, TileId.ROAD);
                    Instantiate(roadPrefab, gridLocation, Quaternion.identity);
                }
                else
                {
                    Debug.Log("ROAD cannot be placed");
                }
            }
        }
    }

    public Vector3 ToGridPos(Vector3 originalPos)
    {
        return new Vector3(Mathf.Round(originalPos.x / gridSize) * gridSize, 0.02f, Mathf.Round(originalPos.z / gridSize) * gridSize);
    }
    
    public Vector2Int VectorToTile(Vector3 originalPos)
    {
        return new Vector2Int(Mathf.FloorToInt(originalPos.x / gridSize), Mathf.FloorToInt(originalPos.z / gridSize));
    }

    public Vector3 TileToCenterWorld(Vector2Int originalPos)
    {
        return new Vector3(originalPos.x * gridSize, 1f, originalPos.y * gridSize);
    }

    public void SetTile(int x, int y, TileId newTile)
    {
        tile[x + 25, y + 25] = newTile;
    }

    public bool CanBePlaced(int x, int y, TileId tile)
    {
        if (GetTile(x, y) != TileId.EMPTY)
            return false;

        if (GetTile(x + 1, y + 1) == TileId.ROAD && GetTile(x, y + 1) == TileId.ROAD && GetTile(x + 1, y) == TileId.ROAD)
            return false;
        if (GetTile(x - 1, y - 1) == TileId.ROAD && GetTile(x - 1, y) == TileId.ROAD && GetTile(x, y - 1) == TileId.ROAD)
            return false;
        if (GetTile(x + 1, y - 1) == TileId.ROAD && GetTile(x, y - 1) == TileId.ROAD && GetTile(x + 1, y) == TileId.ROAD)
            return false;
        if (GetTile(x - 1, y + 1) == TileId.ROAD && GetTile(x - 1, y) == TileId.ROAD && GetTile(x, y + 1) == TileId.ROAD)
            return false;

        return true;
    }

    public TileId GetTile(int x,int y)
    {
        if (x + 25 >= 100 || y + 25 >= 100 || x < -25 || y < -25)
            return TileId.EMPTY;

        return tile[x + 25, y + 25];
    }

    public TileId GetTile(Vector2Int pos)
    {
        return GetTile(pos.x, pos.y);
    }

    public bool IsBranchingTile(int curX, int curY, int tileX, int tileY)
    {
        return GetAdjacentTiles(curX, curY, tileX, tileY).Count > 1;
    }

    public List<Vector2Int> GetAdjacentTiles(int curX, int curY, int tileX, int tileY)
    {
        List<Vector2Int> availableTiles = new List<Vector2Int>();
        Vector2Int[] direction = new Vector2Int[4] { new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1) };

        for (int i = 0; i < 4; i++)
        {
            Vector2Int newPos = new Vector2Int(tileX + direction[i].x, tileY + direction[i].y);
            if (GetTile(newPos) == TileId.ROAD && !(newPos.x == curX && newPos.y == curY))
                availableTiles.Add(newPos);
        }

        return availableTiles;
    }

    private void OnDrawGizmosSelected()
    {
        if (gridSize <= 8)
            gridSize = 8;

        for (int x = -25; x <= 25; x++)
        {
            for (int y = -25; y <= 25; y += 2)
            {
                if (CarTrafficManager.Instance.GetCarIndex(x, y) != 0)
                {
                    Gizmos.color = Color.red;
                } else
                {
                    Gizmos.color = Color.yellow;
                }
                Gizmos.DrawWireCube(new Vector3(x * gridSize, 0.02f, y * gridSize), new Vector3(gridSize, 1, gridSize));
                if (CarTrafficManager.Instance.GetCarIndex(y, x) != 0)
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.yellow;
                
                Gizmos.DrawWireCube(new Vector3(y * gridSize, 0.02f, x * gridSize), new Vector3(gridSize, 1, gridSize));
            }
        }
        
    }

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one TileManager instance!");
            Destroy(this);
            return;
        }
        Instance = this;
    }
}
