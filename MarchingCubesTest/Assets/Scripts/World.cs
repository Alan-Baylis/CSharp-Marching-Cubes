using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{

    public int worldWidth = 4;
    public int worldDepth = 4;
    public int worldHeight = 4;

    public int chunkSize = 8;

    public float noiseScale = 10;

    [Range(0f, 1f)]
    public float isolevel = 0.5f;

    public bool interpolate = true;

    public Chunk[,,] chunks;

    public GameObject chunkPrefab;

    public GameObject playerGo;
    public Player player;

    public static World instance;

    void Awake()
    {
        if (!instance) instance = this;
        else Destroy(gameObject);

        chunks = new Chunk[worldWidth, worldHeight, worldDepth];

    }

    void Start()
    {
        playerGo = Player.instance.gameObject;
        player = playerGo.GetComponent<Player>();

        for (int x = 0; x < worldWidth; x++)
        {
            for (int y = 0; y < worldHeight; y++)
            {
                for (int z = 0; z < worldDepth; z++)
                {
                    GameObject chunkGo = Instantiate(chunkPrefab, new Vector3(x * chunkSize, y * chunkSize, z * chunkSize), Quaternion.identity);
                    chunkGo.name = x + " " + y + " " + z;

                    Chunk chunk = chunkGo.GetComponent<Chunk>();

                    chunk.pos = new Vector3Int(x * chunkSize, y * chunkSize, z * chunkSize);

                    chunk.noiseScale = noiseScale;
                    chunk.chunkSize = chunkSize;

                    chunk.worldWidth = worldWidth;
                    chunk.worldDepth = worldDepth;
                    chunk.worldHeight = worldHeight;

                    chunk.force = player.force;
                    chunk.add = player.add;
                    chunk.range = player.range;
                    chunk.forceOverDistance = player.forceOverDistance;

                    chunks[x, y, z] = chunk;
                }
            }
        }

        for (int x = 0; x < worldWidth; x++)
        {
            for (int y = 0; y < worldHeight; y++)
            {
                for (int z = 0; z < worldDepth; z++)
                {

                    Chunk chunk = chunks[x, y, z];

                    for (int i1 = -1; i1 < 2; i1++)
                    {
                        for (int i2 = -1; i2 < 2; i2++)
                        {
                            for (int i3 = -1; i3 < 2; i3++)
                            {
                                if (x - i1 >= 0 && x - i1 < worldWidth && y - i2 >= 0 && y - i2 < worldHeight && z - i3 >= 0 && z - i3 < worldDepth)
                                {
                                    chunk.neighbours[i1 + 1, i2 + 1, i3 + 1] = chunks[x - i1, y - i2, z - i3];
                                }
                            }
                        }
                    }
                }
            }
        }

        System.GC.Collect();
    }

    public Chunk FindChunk(int x, int y, int z)
    {

        Chunk chunk = chunks[(x / chunkSize), (y / chunkSize), (z / chunkSize)];

        return chunk;
    }

    public Voxel FindVoxel(int x, int y, int z)
    {

        return FindChunk(x, y, z).FindVoxel(x,y,z);
    }
}


