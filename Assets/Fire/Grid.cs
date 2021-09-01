using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public ComputeBuffer temperature;
    public ComputeBuffer temperatureNew;

    //we split the X,Y and Z component of the velocity because of the MAC GRID
    public ComputeBuffer velocityX;
    public ComputeBuffer velocityY;
    public ComputeBuffer velocityZ;

    public ComputeBuffer velocityNew;

    public ComputeBuffer vorticity;

    public ComputeBuffer smokeDensity;
    public ComputeBuffer smokeDensityNew;

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
        temperature = new ComputeBuffer(flatten(gridSize), sizeof(float));
        temperatureNew = new ComputeBuffer(flatten(gridSize), sizeof(float));
        velocityX = new ComputeBuffer(flattenPlusOne(gridSize), sizeof(float));
        velocityY = new ComputeBuffer(flattenPlusOne(gridSize), sizeof(float));
        velocityZ = new ComputeBuffer(flattenPlusOne(gridSize), sizeof(float));
        velocityNew = new ComputeBuffer(flattenPlusOne(gridSize), sizeof(float) * 4);
        vorticity = new ComputeBuffer(flattenPlusOne(gridSize), sizeof(float) * 4);
        smokeDensity = new ComputeBuffer(flatten(gridSize), sizeof(float));
        smokeDensityNew = new ComputeBuffer(flatten(gridSize), sizeof(float));
        pressure = new ComputeBuffer(flatten(gridSize), sizeof(float));

    }

    //clear all the buffers (aka free the gpu of data)
    public void clear()
    {
        temperature.Release();
        temperatureNew.Release();
        velocityX.Release();
        velocityY.Release();
        velocityZ.Release();
        velocityNew.Release();
        vorticity.Release();
        smokeDensity.Release();
        smokeDensityNew.Release();
    }

    public void setBuffers(ComputeShader shader)
    {
        shader.SetBuffer(0, "smokeDensity", smokeDensity);
        shader.SetBuffer(0, "smokeDensityNew", smokeDensityNew);
        shader.SetBuffer(0, "temperature", temperature);
        shader.SetBuffer(0, "temperatureNew", temperatureNew);
        shader.SetBuffer(0, "velocityX", velocityX);
        shader.SetBuffer(0, "velocityY", velocityY);
        shader.SetBuffer(0, "velocityZ", velocityZ);
        shader.SetBuffer(0, "velocityNew", velocityNew);
        //shader.SetBuffer(0, "vorticity", vorticity);
        //shader.SetBuffer(0, "pressure", pressure);

    }
}
