using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public ComputeBuffer TempAndDensity;

    public ComputeBuffer velocity;

    public ComputeBuffer velocityNew;

    public ComputeBuffer vorticity;

    public ComputeBuffer pressure;

    private Vector3Int gridSize;

    int flatten(Vector3Int vector)
    {
        return vector.x * vector.y * vector.z;
    }

    int flattenPlusOne(Vector3Int vector)
    {
        return (vector.x+1) * (vector.y+1) * (vector.z+1);
    }


    //init each buffer of the domain
    public void create(Vector3Int gs)
    {
        gridSize = gs;
        TempAndDensity = new ComputeBuffer(flatten(gridSize), sizeof(float) * 4);
        velocity = new ComputeBuffer(flattenPlusOne(gridSize), sizeof(float) * 4);
        velocityNew = new ComputeBuffer(flattenPlusOne(gridSize), sizeof(float) * 4);
        vorticity = new ComputeBuffer(flattenPlusOne(gridSize), sizeof(float) * 4);
        pressure = new ComputeBuffer(flatten(gridSize), sizeof(float));

    }

    //clear all the buffers (aka free the gpu of data)
    public void clear()
    {
        TempAndDensity.Release();
        velocity.Release();
        pressure.Release();
        velocityNew.Release();
        vorticity.Release();
    }

    public void setBuffers(ComputeShader shader)
    {
        shader.SetBuffer(0, "velocity", velocity);
        shader.SetBuffer(0, "TempAndDensity", TempAndDensity);
        shader.SetBuffer(0, "velocityNew", velocityNew);
        shader.SetBuffer(0, "vorticity", vorticity);
        shader.SetBuffer(0, "pressure", pressure);

    }
}
