using System;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private float rotationRadius = 8f;
    private Vector2Int lastTilePosition = new Vector2Int(-250,-250);

    public float speed = 5f;

    [SerializeField]
    private int carIndex = -1;

    private Vector3 nextTileWorldPosition;
    private Vector3 branchPosition, lastPosition;
    private bool shouldRotateToPoint = false;
     
    void Start()
    {
        Vector2Int currentTilePosition = TileManager.Instance.VectorToTile(transform.position);
        carIndex = CarTrafficManager.Instance.RegisterCar(currentTilePosition.x, currentTilePosition.y);
        NewPosition();
    }

    void Update()
    {
        Vector2Int currentTilePosition = TileManager.Instance.VectorToTile(transform.position);
        Vector2Int nextTilePos = TileManager.Instance.VectorToTile(nextTileWorldPosition);

        
        
        if (transform.position.x == nextTileWorldPosition.x && transform.position.z == nextTileWorldPosition.z)
        {
            branchPosition = new Vector3(-250, -250, -250);
            lastPosition = new Vector3(-250, -250, -250);
            NewPosition();
            
        } else if (CarTrafficManager.Instance.GetCarIndex(nextTilePos) == carIndex || CarTrafficManager.Instance.GetCarIndex(nextTilePos) == 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, nextTileWorldPosition, speed * Time.deltaTime);
        } else
        {
            var nextTilePosition = TileManager.Instance.VectorToTile(nextTileWorldPosition);
            nextTileWorldPosition = TileManager.Instance.TileToCenterWorld(lastTilePosition);
            lastTilePosition = nextTilePosition;
        }

        if (shouldRotateToPoint)
        {
            if (lastPosition.x == -250)
            {
                lastPosition = transform.position;
            }

            // lastPosition = position of start piece
            // branchPosition = position of intersection piece
            // nextTileWorldPosition = position of end piece
        }
        CarTrafficManager.Instance.UpdateCarPosition(carIndex, currentTilePosition);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(TileManager.Instance.ToGridPos(transform.position), new Vector3(10, 10, 10));
        Gizmos.DrawWireSphere(nextTileWorldPosition, .5f);

        if (lastPosition.x != -250)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(lastPosition, .5f);
        }

        Vector2Int nextTilePos = TileManager.Instance.VectorToTile(nextTileWorldPosition);
        if (CarTrafficManager.Instance.GetCarIndex(nextTilePos) == carIndex || CarTrafficManager.Instance.GetCarIndex(nextTilePos) == 0)
        {
            Gizmos.color = Color.green;
        } else
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawLine(transform.position, nextTileWorldPosition);

    }

    // Update is called once per frame
    void NewPosition()
    {
        Vector2Int tilePosition = TileManager.Instance.VectorToTile(transform.position);
        int x = tilePosition.x, y = tilePosition.y;

        // get next tile.
        List<Vector2Int> availableTiles = new List<Vector2Int>();
        Vector2Int[] direction = new Vector2Int[4] { new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1) };

        for (int i = 0; i < 4; i++)
        {
            Vector2Int newPos = new Vector2Int(x + direction[i].x, y + direction[i].y);
            if (TileManager.Instance.GetTile(newPos) == TileManager.TileId.ROAD && !(newPos.x == lastTilePosition.x && newPos.y == lastTilePosition.y))
                availableTiles.Add(newPos);
        }

        Vector3 tileCenter = transform.position;
        if (availableTiles.Count >= 1)
        {
            Vector2Int newTilePosition = availableTiles[UnityEngine.Random.Range(0, availableTiles.Count)];
            List<Vector2Int> branchPaths = TileManager.Instance.GetAdjacentTiles(x, y, newTilePosition.x, newTilePosition.y);
            if (branchPaths.Count > 1)
            {
                // TEMP: Go to this random tile instead.
                shouldRotateToPoint = true;
                branchPosition = TileManager.Instance.TileToCenterWorld(newTilePosition);
                newTilePosition = branchPaths[UnityEngine.Random.Range(0, branchPaths.Count)];
            }
                tileCenter = TileManager.Instance.TileToCenterWorld(newTilePosition);
        } else if (lastTilePosition.x > -240 && lastTilePosition.y > -240)
        {
            tileCenter = TileManager.Instance.TileToCenterWorld(lastTilePosition);
        } else
        {
            // car is stuck
            Debug.Log("Car is stuck");
        }
        lastTilePosition = tilePosition;
        nextTileWorldPosition = tileCenter;
        Debug.Log(tileCenter);
    }
}
