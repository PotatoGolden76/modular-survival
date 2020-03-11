using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkSorter : MonoBehaviour
{

    void Awake()
    {
        if(gameObject.transform.position.y > 0)
        {
            gameObject.transform.position = gameObject.transform.position + new Vector3(0, 0, 0.1f * gameObject.transform.position.y / Chunk.CHUNK_SIZE_Y);
        } else
        {
            gameObject.transform.position = gameObject.transform.position + new Vector3(0, 0, 0.1f * gameObject.transform.position.y / Chunk.CHUNK_SIZE_Y);
        }
        Destroy(this);
    }

}
