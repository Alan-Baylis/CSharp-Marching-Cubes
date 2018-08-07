/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class Voxel
{

    public GridCell cell;
    public Triangle[] triangles;

    public Chunk chunk;

    private bool cornersCreated = false;

    public bool densitiesCreated = false;

    public int x;
    public int y;
    public int z;



    public Voxel(int x, int y, int z, Chunk chunk)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.chunk = chunk;

        cell.densities = new float[8];
        cell.corners = new Vector3Int[8];

        if (!cornersCreated)
            GenerateCorners();
        GenerateDensities();
    }

	public void March(float isolevel, int chunkSize, bool interpolate, List<Vector3> verts){
	
		
        GenerateDensities();

        if (triangles == null)
            triangles = new Triangle[5];

        int triCount = MarchingCubes.Polygonise(cell, isolevel, triangles, interpolate);

        for (int i = 0; i < triCount; i++)
        {
            AddVerts(verts, i);

        }
    }

    void AddVerts(List<Vector3> verts, int i)
    {
        verts.Add(triangles[i].points[0]);
        verts.Add(triangles[i].points[1]);
        verts.Add(triangles[i].points[2]);

        //verts.AddRange(triangles[i].points);

        chunk.j += 3;
    }


    public void GenerateDensities()
    {

        //if (densitiesCreated)
        //{
            for (int i = 0; i < cell.corners.Length; i++)
            {
            Vector3Int corner = cell.corners[i];
                cell.densities[i] = chunk.GetDensity(corner.x, corner.y, corner.z);

            }
        //}
        //else
        //{

        //    cell.densities = new float[8];
        //    for (int i = 0; i < corners.Length; i++)
        //    {
        //        cell.densities[i] = chunk.GetDensity(corners[i].x, corners[i].y, corners[i].z);
        //        chunk.densities[corners[i].x, corners[i].y, corners[i].z] = cell.densities[i];
        //    }
        //    densitiesCreated = true;
        //}
    }


    public void GenerateCorners()
    {

        cell.corners = new Vector3Int[8];


        cell.corners[0] = new Vector3Int(x, y, z);
        cell.corners[1] = new Vector3Int(x + 1, y, z);
        cell.corners[2] = new Vector3Int(x + 1, y, z + 1);
        cell.corners[3] = new Vector3Int(x, y, z + 1);
        
        cell.corners[4] = new Vector3Int(x, y + 1, z);
        cell.corners[5] = new Vector3Int(x + 1, y + 1, z);
        cell.corners[6] = new Vector3Int(x + 1, y + 1, z + 1);
        cell.corners[7] = new Vector3Int(x, y + 1, z + 1);

        cornersCreated = true;


    }
}
*/