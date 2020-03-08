using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//temp
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public Rigidbody2D rb;
   
    Vector2 movement;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();   
    }

    // Update is called once per frame
    void Update()
    {
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        movement.Normalize();

        movement *= speed;

        if (Input.GetMouseButtonDown(0))
        {

            //not working

            Ray r = gameObject.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if(Physics.Raycast(r, out hit, LayerMask.GetMask("Chunk")))
            {
                Debug.Log(hit.transform.parent.transform.parent.GetComponent<Tilemap>().GetTile(new Vector3Int(7, 7, 0)));
            } else
            {
                Debug.Log("coll null");
            }

        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
    }

}
