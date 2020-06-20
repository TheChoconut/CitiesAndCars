using System;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody rigidbody;

    private float rotationRadius = 10f;
    public Vector2Int lastTilePosition = new Vector2Int(-250,-250);

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        InvokeRepeating("NewPosition", 0f, 0.25f);
    }

    void Start()
    {

    }

    void Update()
    {
        float progress = Time.time % (90 * Mathf.Deg2Rad) / (90 * Mathf.Deg2Rad);

        //transform.position = new Vector3((float)Math.Cos(Time.time % (90 * Mathf.Deg2Rad)) * rotationRadius, transform.position.y, (float)Math.Sin(Time.time % (90 * Mathf.Deg2Rad)) * rotationRadius);
        //transform.eulerAngles = new Vector3(transform.eulerAngles.x, 90 - progress * 90, transform.eulerAngles.z);
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
            tileCenter = TileManager.Instance.TileToCenterWorld(newTilePosition);
        } else if (lastTilePosition.x > -250 && lastTilePosition.y > -250)
        {
            tileCenter = TileManager.Instance.TileToCenterWorld(lastTilePosition);
        } else
        {
            // car is stuck
        }
        lastTilePosition = tilePosition;
        transform.position = tileCenter;
    }
}
