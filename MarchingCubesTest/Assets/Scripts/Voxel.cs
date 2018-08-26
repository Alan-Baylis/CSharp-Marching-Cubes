using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Voxel {

    public Vector3Int voxelPositionGlobal;
    public Vector3Int voxelPositionChunk;

    public float[] densities;
    public Vector3Int[] globalCorners;
    public Vector3Int[] corners;

    public Chunk chunk;

    public Voxel(Vector3Int voxelPositionGlobal, Chunk chunk)
    {
        this.voxelPositionGlobal = voxelPositionGlobal;
        this.voxelPositionChunk = new Vector3Int(this.voxelPositionGlobal.x % chunk.chunkSize, 
                                                 this.voxelPositionGlobal.y % chunk.chunkSize, 
                                                 this.voxelPositionGlobal.z % chunk.chunkSize);

        this.densities = new float[8];
        this.chunk = chunk;

        corners = new Vector3Int[8];
        globalCorners = new Vector3Int[8];
        GenerateCorners();

        SetDefaulDensities();

    }

    public Voxel(Vector3Int voxelPositionGlobal, float[] densities, Chunk chunk)
    {
        this.voxelPositionGlobal = voxelPositionGlobal;
        this.voxelPositionChunk = new Vector3Int(this.voxelPositionGlobal.x % chunk.chunkSize,
                                                 this.voxelPositionGlobal.y % chunk.chunkSize,
                                                 this.voxelPositionGlobal.z % chunk.chunkSize);

        this.densities = densities;
        this.chunk = chunk;

        corners = new Vector3Int[8];
        globalCorners = new Vector3Int[8];

        GenerateCorners();
    }

    void SetDefaulDensities()
    {
        for(int i = 0; i<8; i++)
        {
            Vector3Int densityPos = globalCorners[i];

            densities[i] = chunk.CalculateDensity(densityPos.x, densityPos.y, densityPos.z);
        }
    }

    void GenerateCorners()
    {
        Vector3Int p = voxelPositionGlobal;
        
        
        int x = p.x;
        int y = p.y;
        int z = p.z;
        
        globalCorners[0] = new Vector3Int(x, y, z);
        globalCorners[1] = new Vector3Int(x + 1, y, z);
        globalCorners[2] = new Vector3Int(x + 1, y, z + 1);
        globalCorners[3] = new Vector3Int(x, y, z + 1);
        globalCorners[4] = new Vector3Int(x, y + 1, z);
        globalCorners[5] = new Vector3Int(x + 1, y + 1, z);
        globalCorners[7] = new Vector3Int(x, y + 1, z + 1);
        globalCorners[6] = new Vector3Int(x + 1, y + 1, z + 1);

        p = voxelPositionChunk;

        x = p.x;
        y = p.y;
        z = p.z;

        corners[0] = new Vector3Int(x, y, z);
        corners[1] = new Vector3Int(x + 1, y, z);
        corners[2] = new Vector3Int(x + 1, y, z + 1);
        corners[3] = new Vector3Int(x, y, z + 1);
        corners[4] = new Vector3Int(x, y + 1, z);
        corners[5] = new Vector3Int(x + 1, y + 1, z);
        corners[7] = new Vector3Int(x, y + 1, z + 1);
        corners[6] = new Vector3Int(x + 1, y + 1, z + 1);
    }
}
