using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Domain : MonoBehaviour
{

    public Vector3Int blockCount;
    public Vector3 blockSize;
    public GameObject domain;

    private float[] temperature;
    private Vector3[] velocity;
    private float[] smokeDensity;

    // Start is called before the first frame update
    public void create()
    {
        domain.transform.localScale = new Vector3(blockCount.x / blockSize.x, blockCount.y / blockSize.y, blockCount.z / blockSize.z);

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