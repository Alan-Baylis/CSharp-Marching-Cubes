using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	public float iso = 0.5f;	

	public float force;

    public AnimationCurve forceOverDistance;

	public bool add = true;

    private bool voxelsCreated = false;

    public Voxel[,,] voxels;

    [HideInInspector]
    public int j = 0;

	public float GetDensity(float x, float y, float z){

		if(densitiesCreated[(int)x,(int)y,(int)z])
			return densities[(int)x,(int)y,(int)z];
        else
		    return CalculateDensity(x,y,z);

 		

	}

    public float CalculateDensity(float x, float y, float z)
    {
        densitiesCreated[(int)x, (int)y, (int)z] = true;

        return -1 * x * y * z + 30;
    }

	public float PerlinNoise3D(float x, float y, float z){

		float _x = chunkSize/(x+1);
		float _y = chunkSize/(y+1);
		float _z = chunkSize/(z+1);

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

	void OnValidate(){
		// UpdateMesh();
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
        voxels = new Voxel[chunkSize - 1, chunkSize - 1, chunkSize - 1];

		UpdateMesh();
	}

	void Update(){

		if(Input.GetButtonDown("Fire1")){
            if (force > 0)
            {

                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {

                    Chunk chunk = hit.transform.GetComponent<Chunk>();

                    chunk.ModifyTerrain(hit.point, add, force);
                }
            }
		}
	}

    void ModifyTerrain(Vector3 p, bool build, float force)
    { 

        //Calculate modification range
        int range = Mathf.CeilToInt(force / 2f);

        int modifier;

        //Choose modifier: 
        //Build  =   1  
        //Break  =  -1
        if (build)
            modifier = 1;
        else
            modifier = -1;
        
        //Loop through all density points in range
        for (int x = -range; x < range; x++)
        {
            for (int y = -range; y < range; y++)
            {
                for (int z = -range; z < range; z++)
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
            }
        }

        UpdateMesh();
    }

    public static float Map(float value, float fromSource, float toSource, float fromTarget, float toTarget)
    {
        return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
    }


    void UpdateMesh(){

        List<Vector3> verts = new List<Vector3>();

        for (int x = 0; x<chunkSize-1; x++){
			for(int y = 0; y<chunkSize-1; y++){
				for(int z = 0; z<chunkSize-1; z++){

                    Voxel voxel;

                    if (!voxelsCreated)
                    {
                        voxel = new Voxel(x, y, z, this);
                        voxels[x, y, z] = voxel;
                    }
                    else
                        voxel = voxels[x, y, z];

					voxel.March(iso, chunkSize, interpolate, verts);


				}
			}
		}

        voxelsCreated = true;

		int[] tris = new int[j];

		for(int i = 0; i<j; i++){
			tris[i] = i;
		}

        mesh.Clear(false);

        mesh.SetVertices(verts);
		mesh.triangles = tris;

		mesh.RecalculateNormals();

		meshFilter.mesh = mesh;

        j = 0;

		GetComponent<MeshCollider>().sharedMesh = mesh;


	}
}
