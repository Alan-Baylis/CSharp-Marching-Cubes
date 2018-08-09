using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class Chunk : MonoBehaviour {


	public int chunkSize = 16;

    [HideInInspector]
	public MeshFilter meshFilter;

    [HideInInspector]
	public Mesh mesh;

	public bool interpolate;

	public float[,,] densities;
    public bool[,,] densitiesCreated;

	[Range(0f,1f)]
	public float isolevel = 0.5f;

    public float noiseScale = 0.3f;

    public float force;

    [Header("Set to 0 if you want to use default range")]
    public int range;

    public AnimationCurve forceOverDistance;

	public bool add = true;

    public int xOff;
    public int yOff;
    public int zOff;


    //public Voxel[,,] voxels;


    [HideInInspector]
    public int j = 0;

    public float GetDensity(int x, int y, int z)
    {

        float density = densities[x,y,z];
        
        if(!densitiesCreated[x,y,z])
        {
            densitiesCreated[x, y, z] = true;
            density = CalculateDensity(x+xOff, y+yOff, z+zOff);
            densities[x, y, z] = density;
            return density;
        }

        return density;
    }

    public float CalculateDensity(int x, int y, int z)
    {

        float density = Mathf.Clamp01(y - Mathf.PerlinNoise(chunkSize / (x + 1f) / noiseScale, chunkSize / (z + 1f) / noiseScale));

        return density;
    }

    public float PerlinNoise2D(float x, float y)
    {
        float _x = chunkSize / (x + 1);
        float _y = chunkSize / (y + 1);

        float val = Mathf.PerlinNoise(_x, _y);

        return val;
    }

	public float PerlinNoise3D(float x, float y, float z){

		float _x = ((x + 1) / 5f);
        float _z = ((z + 1) / 5f);
        float _y = ((y + 1) / 5f);

        float AB = Mathf.PerlinNoise(_x, _y);
		float BC = Mathf.PerlinNoise(_y, _z);
		float AC = Mathf.PerlinNoise(_x, _z);

		float BA = Mathf.PerlinNoise(_y, _x);
		float CB = Mathf.PerlinNoise(_z, _y);
		float CA = Mathf.PerlinNoise(_z, _x);

		float ABC = AB + BC + AC + BA + CB + CA;
		
		float val = ABC / 6f;

		return val;
	}

    void UpdateMesh()
    {

        MarchingCubes.Polygonise(mesh, xOff, yOff, zOff, chunkSize, isolevel, densities, interpolate);

        meshFilter.mesh = mesh;

        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    void Awake(){
		densities = new float[chunkSize, chunkSize, chunkSize];
        densitiesCreated = new bool[chunkSize, chunkSize, chunkSize];

        mesh = new Mesh();

		meshFilter = GetComponent<MeshFilter>();
	}

	// Use this for initialization
	void Start () {

		gameObject.AddComponent<MeshCollider>();

        for (int x = 0; x < chunkSize; x++)
        for (int y = 0; y < chunkSize; y++)
        for (int z = 0; z < chunkSize; z++)
        {
            densities[x, y, z] = GetDensity(x, y, z);
        }

        UpdateMesh();
	}

	void Update(){

        Vector3 pos = transform.position;

        xOff = (int)pos.x;
        yOff = (int)pos.y;
        zOff = (int)pos.z;

        


		if(Input.GetButton("Fire1")){
            if (force > 0)
            {

                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {

                    Chunk chunk = hit.transform.GetComponent<Chunk>();

                    chunk.ModifyTerrain(hit.point, add, force, range);
                }
            }
		}
	}

    void ModifyTerrain(Vector3 p, bool build, float force, int range)
    {
        //Calculate modification range
        int _range = (range == 0 ? Mathf.CeilToInt(force / 2f) : range);

        int modifier;

        //Choose modifier: 
        //Build  =   1  
        //Break  =  -1
        if (build)
            modifier = 1;
        else
            modifier = -1;

        
        //Loop through all density points in range
        for (int x = -_range; x < _range; x++)
        for (int y = -_range; y < _range; y++)
        for (int z = -_range; z < _range; z++)
        {
            //Clamp inside chunk
            //Round, this is quite messy 
            int _x = Mathf.Clamp(Mathf.RoundToInt(p.x) - x, 0, chunkSize - 1);
            int _y = Mathf.Clamp(Mathf.RoundToInt(p.y) - y, 0, chunkSize - 1);
            int _z = Mathf.Clamp(Mathf.RoundToInt(p.z) - z, 0, chunkSize - 1);

            //Calculate distance between density's location and hitpoint
            float distance = Vector3.Distance(new Vector3(_x, _y, _z), p);

            //Change density depending on distance
            densities[_x, _y, _z] -= (force / distance * forceOverDistance.Evaluate(Map(distance, 0, force, 0, 1))) * modifier;
        }

        UpdateMesh();
    }

    public static float Map(float value, float fromSource, float toSource, float fromTarget, float toTarget)
    {
        return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
    }
}
