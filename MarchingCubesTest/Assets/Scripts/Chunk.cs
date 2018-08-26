using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class Chunk : MonoBehaviour
{

    [HideInInspector] public MeshFilter meshFilter;

    [HideInInspector] public Mesh mesh;

    public Vector3Int pos;

    public int chunkSize;
    public float noiseScale;

    public int worldWidth;
    public int worldDepth;
    public int worldHeight;

    public float force;
    public bool add;
    public int range;
    public AnimationCurve forceOverDistance;

    public World world;
    public Player player;

    public Voxel[,,] voxels;

    public Chunk[,,] neighbours = new Chunk[3, 3, 3];


    void Start()
    {
        voxels = new Voxel[chunkSize, chunkSize, chunkSize];

        world = World.instance;
        player = Player.instance;

        mesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        gameObject.AddComponent<MeshCollider>();

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    voxels[x, y, z] = new Voxel(new Vector3Int(x + pos.x, y + pos.y, z + pos.z), this);
                }
            }
        }

        UpdateMesh();
    }

    public float CalculateDensity(float x, float y, float z)
    {

        float density = PerlinNoise3D(x, y, z); //y - Mathf.PerlinNoise( / (x + 1f) / noiseScale, chunkSize / (z + 1f) / noiseScale));

        return density;
    }

    public float PerlinNoise2D(float x, float y)
    {
        float _x = chunkSize / (x + 1);
        float _y = chunkSize / (y + 1);

        float val = Mathf.PerlinNoise(_x, _y);

        return val;
    }

    public float PerlinNoise3D(float x, float y, float z)
    {

        float _z = ((z + 1f) / noiseScale);
        float _y = ((y + 1f) / noiseScale);
        float _x = ((x + 1f) / noiseScale);

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

    public Voxel FindVoxel(int x, int y, int z)
    {
        return voxels[x % chunkSize, y % chunkSize, z % chunkSize];
    }

    public void UpdateMesh()
    {

        MarchingCubes.Polygonise(ref mesh, voxels, pos, chunkSize, world.isolevel, world.interpolate);

        meshFilter.mesh = mesh;

        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}
