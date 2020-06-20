using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTrafficManager : MonoBehaviour
{

    private List<int> Cars;

    private Dictionary<int, Vector2Int> CarPosition;

    public static CarTrafficManager Instance;

    int[,] carMatrix = new int[100, 100];

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            Debug.LogWarning("More than one CarTrafficManager! Destroying object");
        }
        Instance = this;
        CarPosition = new Dictionary<int, Vector2Int>();
        Cars = new List<int>();
    }

    public int RegisterCar(int x, int y)
    {
        Cars.Add(Cars.Count + 1);
        CarPosition[Cars.Count] = new Vector2Int(x, y);
        carMatrix[x + 25, y + 25] = Cars.Count;
        return Cars.Count;
    }

    public void UpdateCarPosition(int carIndex, int x, int y)
    {
        Vector2Int lastPos = CarPosition[carIndex];
        if (lastPos.x == x && lastPos.y == y)
            return;
        carMatrix[lastPos.x + 25, lastPos.y + 25] = 0;
        CarPosition[carIndex] = new Vector2Int(x,y);
        carMatrix[x + 25, y + 25] = carIndex;
    }

    public void UpdateCarPosition(int carIndex, Vector2Int pos)
    {
        UpdateCarPosition(carIndex, pos.x, pos.y);
    }
    
    public int GetCarIndex(int x, int y)
    {
        return carMatrix[x + 25, y + 25];
    }

    public int GetCarIndex(Vector2Int pos)
    {
        return GetCarIndex(pos.x, pos.y);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
