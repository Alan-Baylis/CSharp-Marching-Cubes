using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float force = 1f;
    public bool add;
    public int range;
    public AnimationCurve forceOverDistance;

    public bool RedditWay = true;

    public static Player instance;
    public World world;

    void Awake()
    {
        if (!instance) instance = this;
        else Destroy(gameObject);

    }

    void Start()
    {
        world = World.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (force > 0)
            {

                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    ModifyTerrain(hit.point, add, force, range);
                }
            }
        }
    }

    public void ModifyTerrain(Vector3 p, bool build, float force, int range)
    {
        //Calculate modification range
        int _range = (range == 0 ? Mathf.CeilToInt(force / 2f) : range);

        if (_range <= 0)
            return;

        float modificationAmount;

        List<Chunk> modifiedChunks = new List<Chunk>();


        //Loop through all density points in range
        for (int x = -_range; x < _range; x++)
        {
            for (int y = -_range; y < _range; y++)
            {
                for (int z = -_range; z < _range; z++)
                {

                    int _x = (Mathf.RoundToInt(p.x) - x);
                    int _y = (Mathf.RoundToInt(p.y) - y);
                    int _z = (Mathf.RoundToInt(p.z) - z);


                    

                    Chunk chunk = world.FindChunk(_x, _y, _z);
                    Voxel voxel = chunk.FindVoxel(_x, _y, _z);

                    if(!modifiedChunks.Contains(chunk))
                        modifiedChunks.Add(chunk);

                    for (int i = 0; i < 8; i++)
                    {

                        Vector3Int densityLocation = voxel.globalCorners[i];

                        //Calculate distance between density's location and hitpoint
                        float distance = Vector3.Distance(densityLocation, p);

                        if (build)
                            modificationAmount = (force / distance * forceOverDistance.Evaluate(Utils.Map(distance, 0, force, 0, 1))) * 1f;
                        else
                            modificationAmount = (force / distance * forceOverDistance.Evaluate(Utils.Map(distance, 0, force, 0, 1))) * -1f;

                        voxel.densities[i] = Mathf.Max(voxel.densities[i]+modificationAmount, voxel.densities[i]);
                        Debug.Log(voxel.densities[i]);

                    }
                }
            }
        }

        for (int i = 0; i < modifiedChunks.Count; i++)
        {
            modifiedChunks[i].UpdateMesh();
        }
    }
}
