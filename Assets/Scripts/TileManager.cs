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
            Vector3 worldPoint = ray.GetPoint(-ray.origin.y / ray.direction.y);
            Vector3 gridLocation = ToGridPos(worldPoint);

            int x = Mathf.RoundToInt(gridLocation.x) / 10, y = Mathf.RoundToInt(gridLocation.z) / 10;
            Debug.Log("ROAD x "+ x+" y " + y);
            if (CanBePlaced(x, y, TileId.ROAD))
            {
                SetTile(x, y, TileId.ROAD);
                Instantiate(roadPrefab, gridLocation, Quaternion.identity);
            } else
            {
                Debug.Log("ROAD cannot be placed");
            }
            
        }
    }

    public Vector3 ToGridPos(Vector3 originalPos)
    {
        return new Vector3(Mathf.Floor(originalPos.x / gridSize) * gridSize, 0.02f, Mathf.Floor(originalPos.z / gridSize) * gridSize);
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
        tile[x + 50, y + 50] = newTile;
    }

    public bool CanBePlaced(int x, int y, TileId tile)
    {
        if (GetTile(x, y) != TileId.EMPTY)
            return false; 


        /*
        if (tile == TileId.ROAD)
        {
            return GetTile(x + 1, y + 1) != TileId.ROAD && GetTile(x - 1, y - 1) != TileId.ROAD && GetTile(x - 1, y + 1) != TileId.ROAD && GetTile(x + 1, y - 1) != TileId.ROAD;
        }*/

        return true;
    }

    public TileId GetTile(int x,int y)
    {
        return tile[x + 50, y + 50];
    }

    public TileId GetTile(Vector2Int pos)
    {
        if (pos.x + 50 >= 100 || pos.y + 50 >= 100 || pos.x < -50 || pos.y < -50)
            return TileId.EMPTY;

        return tile[pos.x + 50, pos.y + 50];
    }

    private void OnDrawGizmosSelected()
    {
        if (gridSize <= 8)
            gridSize = 8;

        Gizmos.color = Color.yellow;
        for (int x = -250; x <= 250; x+= gridSize)
        {
            for (int y = -250; y <= 250; y += gridSize*5)
            {
                Gizmos.DrawWireCube(new Vector3(x, 0.02f, y), new Vector3(gridSize, 1, gridSize));
                Gizmos.DrawWireCube(new Vector3(y, 0.02f, x), new Vector3(gridSize, 1, gridSize));
            }
        }
        
    }

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one TileManager instance!" );
            Destroy(this);
            return;
        }
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
