using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkSorter : MonoBehaviour
{

    void Awake()
    {
        gameObject.transform.position = transform.position + new Vector3(0, 0, Mathf.Clamp(gameObject.transform.position.y , -1f, 1f));
        Destroy(this);
    }

}
