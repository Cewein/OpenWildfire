using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Domain : MonoBehaviour
{
    [HideInInspector]
    public Vector3Int blockCount;
    [HideInInspector]
    public Vector3 gridSize;
    [HideInInspector]
    public Vector3 blockSize;
    [HideInInspector]
    public float[] temperature;
    [HideInInspector]
    public Vector3[] velocity;
    [HideInInspector]
    public float[] smokeDensity;

    // Start is called before the first frame update
    public void create(Vector3Int bc, Vector3 gs)
    {
        blockCount = bc;
        gridSize = gs;
        blockSize = new Vector3(gridSize.x / blockCount.x, gridSize.y / blockCount.y, gridSize.z / blockCount.z);

        int blockTotalCount = blockCount.x * blockCount.y * blockCount.z;
        temperature = new float[blockTotalCount];
        velocity = new Vector3[blockTotalCount];
        smokeDensity = new float[blockTotalCount];
    }

    public int flatten(int x, int y, int z)
    {
        return (int)(x + y * blockCount.x + z * blockCount.y * blockCount.z);
    }

}