using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody rigidbody;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector2Int position = TileManager.Instance.VectorToTile(transform.position);
        TileManager.TileId tile = TileManager.Instance.GetTile(position);
        if (tile == TileManager.TileId.ROAD)
        {
            rigidbody.AddForce(Vector3.forward, ForceMode.Impulse);
        } else if (tile != TileManager.TileId.ROAD)
        {
            //rigidbody.
        }
    }
}
