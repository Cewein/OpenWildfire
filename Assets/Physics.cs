using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Physics : MonoBehaviour
{
    [Header("Simulation parameter")]
    //list of global constant for the simulation
    public float Delta = 0.05f;
    public int GRID_COUNT = 64;
    public float GRID_SIZE = 1.0f;
    public Vector3Int M_i = new Vector3Int(8, 8, 8);
    public float T_AMBIANT = 20.0f;
    public float P_ATM = 0.0f;
    public float BUOY_ALPHA = 0.3f; // SMOKE DENSITY
    public float BUOY_BETA = 0.1f; // TEMPERATURE
    public uint SEMILAGRANGIAN_ITERS = 5;
    public float VORTICITY_EPSILON = 1;
    public float TEMPERATURE_ALPHA = 8e-5f;//8e-5;
    public float TEMPERATURE_GAMMA = -4e-7f;//8e-7;
    public int PRESSURE_JACOBI_ITERATIONS = 10;
    public float SMOKE_EXTINCTION_COEFF = 15e1f;
    public float SMOKE_ALBEDO = 0.7f;
    public int SMOKE_RAY_SQRT_COUNT = 60;
    public Vector3 SMOKE_LIGHT_DIR = new Vector3(1, 0, 0);
    public Vector3 SMOKE_LIGHT_POS = new Vector3(-1, 0, 0);
    public float SMOKE_LIGHT_RADIANCE = 5e0f;
    public float EXTERNAL_FORCE_DELTA = 0.1f;
    public float[] heat_params = new float[] { 0.00500f, 1.0f }; // \kappa 
                                         // heat capacity for constant volume, per volume 

    [Header("Domain and Simulation classes")]

    public Grid GPUGrid;

    [Header("Computing shader")]
    public ComputeShader initPhysicsShader;
    public ComputeShader physicsShader;

    private bool isPaused;


    void setVariables(ComputeShader shader)
    {
        shader.SetInt("unitPerSide", GRID_COUNT);
        shader.SetFloat("unitSize", GRID_SIZE);
        shader.SetFloat("tempAmbiant", T_AMBIANT);
        shader.SetFloat("pressureAtmos", P_ATM);
        shader.SetFloat("buoyAlpha", BUOY_ALPHA);
        shader.SetFloat("buoyBeta", BUOY_BETA);
        shader.SetFloat("iDeltaTime", Time.fixedDeltaTime);
    }

    //allow to init or reset the simulation based on a compute shader
    void initSimulation(ComputeShader shader)
    {
        int numThreadPerAxis = Mathf.CeilToInt((GRID_COUNT - 1) / 4.0f);
        setVariables(shader);
        GPUGrid.setBuffers(shader);
        shader.Dispatch(0, numThreadPerAxis, numThreadPerAxis, numThreadPerAxis);
    }

    // Start is called before the first frame update
    void Start()
    {
        //create both domain one for the GPU and one for the CPU
        GPUGrid.create(new Vector3Int(GRID_COUNT, GRID_COUNT, GRID_COUNT));

        initSimulation(initPhysicsShader);

        isPaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        //pause the simulation if true, resume if false
        if (Input.GetKeyDown(KeyCode.P))
            isPaused = !isPaused;
        if (isPaused) return;

        
        if(Input.GetKeyDown(KeyCode.R))
        {
            //reset the simulation
            initSimulation(initPhysicsShader);
        }
        else
        {
            //get the number of thread for each axis
            int numThreadPerAxis = Mathf.CeilToInt((GRID_COUNT - 1) / 4.0f);

            //set the data for the computer shader
            physicsShader.SetFloat("iTime", Time.time);

            setVariables(physicsShader);

            GPUGrid.setBuffers(physicsShader);
       
            //launch the compute shader
            physicsShader.Dispatch(0, numThreadPerAxis, numThreadPerAxis, numThreadPerAxis);
        }

    }

    private void OnApplicationQuit()
    {
        GPUGrid.clear();
    }
}
