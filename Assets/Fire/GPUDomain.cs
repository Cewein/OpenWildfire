using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUDomain : MonoBehaviour
{
    public ComputeBuffer temperature;
    public ComputeBuffer temperatureNew;
    public ComputeBuffer velocity;
    public ComputeBuffer velocityNew;
    public ComputeBuffer ccvelocity;
    public ComputeBuffer vorticity;
    public ComputeBuffer smokeDensity;
    public ComputeBuffer smokeDensityNew;
    public ComputeBuffer pressure;
    public ComputeBuffer smokeVoxelRadiance;
    public ComputeBuffer smokeVoxelTransparency;

    private Vector3Int gridSize;

    int flatten(Vector3Int vector)
    {
        return vector.x * vector.y * vector.z;
    }

    int flattenPlusOne(Vector3Int vector)
    {
        return (vector.x+1) * (vector.y+1) * (vector.z+1);
    }


    // Start is called before the first frame update
    public void create(Vector3Int gs)
    {
        gridSize = gs;
        temperature = new ComputeBuffer(flatten(gridSize), sizeof(float));
        temperatureNew = new ComputeBuffer(flatten(gridSize), sizeof(float));
        velocity = new ComputeBuffer(flattenPlusOne(gridSize), sizeof(float) * 4);
        velocityNew = new ComputeBuffer(flattenPlusOne(gridSize), sizeof(float) * 4);
        ccvelocity = new ComputeBuffer(flatten(gridSize), sizeof(float) * 4);
        vorticity = new ComputeBuffer(flattenPlusOne(gridSize), sizeof(float) * 4);
        smokeDensity = new ComputeBuffer(flatten(gridSize), sizeof(float));
        smokeDensityNew = new ComputeBuffer(flatten(gridSize), sizeof(float));
        smokeVoxelRadiance = new ComputeBuffer(flatten(gridSize), sizeof(float));
        smokeVoxelTransparency = new ComputeBuffer(flatten(gridSize), sizeof(float));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void clear()
    {
        temperature.Release();
        temperatureNew.Release();
        velocity.Release();
        velocityNew.Release();
        ccvelocity.Release();
        vorticity.Release();
        smokeDensity.Release();
        smokeDensityNew.Release();
        smokeVoxelRadiance.Release();
        smokeVoxelTransparency.Release();
    }
}
