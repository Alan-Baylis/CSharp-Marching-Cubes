using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class Voxel {

	public GridCell cell;
    public Triangle[] triangles;

	public Chunk chunk;

	public Vector3[] corners;

	public float[] densities;

    private bool cornersCreated = false;

	public bool densitiesCreated = false;

	public int x;
	public int y;
	public int z;



	public Voxel(int x, int y, int z, Chunk chunk){
		this.x = x;
		this.y = y;
		this.z = z;
		this.chunk = chunk;

        if(!cornersCreated)
            GenerateCorners();
		densities = GenerateDensities(corners);

	}

	public void March(float isolevel, int chunkSize, bool interpolate, List<Vector3> verts){
	
		cell.corners = corners;
		cell.densities = GenerateDensities(corners);

        if (triangles == null)
            triangles = new Triangle[5];

		int triCount = MarchingCubes.Polygonise(cell, isolevel, triangles, interpolate);

		for(int i = 0; i<triCount; i++){
            AddVerts(verts, i);


            
        }	
	}

    void AddVerts(List<Vector3> verts, int i)
    {
        Vector3[] ps = triangles[i].points;

        verts.AddRange(ps);
        

        chunk.j += 3;

    }

	public float[] GenerateDensities(Vector3[] corners){

		if(densitiesCreated){
			for(int i = 0; i<corners.Length; i++){
				densities[i] = chunk.GetDensity(corners[i].x, corners[i].y, corners[i].z);
				// chunk.densities[(int)corners[i].z, (int)corners[i].y, (int)corners[i].x] = densities[i];

			}
			return densities;

		}
		else{

			float[] _densities = new float[8];
			for(int i = 0; i<corners.Length; i++){
				_densities[i] = chunk.GetDensity(corners[i].x, corners[i].y, corners[i].z);
				chunk.densities[(int)corners[i].x, (int)corners[i].y, (int)corners[i].z] = _densities[i];
			}
			densitiesCreated = true;
			return _densities;
		}
	}

	public void GenerateCorners(){

        corners = new Vector3[8];

		corners[0] = new Vector3(x		,y		,z	);
		corners[1] = new Vector3(x+1f	,y		,z	);
		corners[2] = new Vector3(x+1f	,y		,z+1f);
		corners[3] = new Vector3(x		,y		,z+1f);

		corners[4] = new Vector3(x		,y+1f	,z	);
		corners[5] = new Vector3(x+1f	,y+1f	,z	);
		corners[6] = new Vector3(x+1f	,y+1f	,z+1f);
		corners[7] = new Vector3(x		,y+1f	,z+1f);

        cornersCreated = true;

        
	}
}
