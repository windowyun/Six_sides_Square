using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float speed = 1f;
    [SerializeField] float range = 3f;
    [SerializeField] [Range(0f, 1f)] float startpoint = 0f;
    [SerializeField] bool usedirectStartPoint = false;
    [SerializeField] Vector3 directStartPoint;

    float nextMove = 1f;

    Vector3 rememberPosition;
    Vector3 frontPosition;

    Rigidbody rigid2;

    //int xyz = 0;

    void Start()
    {
        rigid2 = GetComponent<Rigidbody>();
        rememberPosition = transform.position;
        frontPosition = rememberPosition + (transform.right * range);

        if (usedirectStartPoint)
            transform.position = directStartPoint;
        else
            transform.position += transform.right * range * startpoint; 
        
        /*
        if (transform.right.x != 0)
            xyz = 0;
        if (transform.right.y != 0)
            xyz = 1;
        if (transform.right.z != 0)
            xyz = 2;
        */
    }

    void Update()
    {
        //if (transform.position[xyz] >= frontPosition[xyz])
        //    nextMove = -1f;

        //else if (transform.position[xyz] <= rememberPosition[xyz])
        //    nextMove = 1f;

        if(transform.position == frontPosition)
        {
            nextMove = -1f;
        }

        else if (transform.position == rememberPosition)
        {
            nextMove = 1f;
        }
    }

    void FixedUpdate()
    {
        //rigid2.velocity = new Vector3(nextMove * speed, rigid2.velocity.y, 0f);
        //transform.Translate(transform.right * nextMove * speed * 0.1f, Space.World);

        if(nextMove >= 1)
        {
            transform.position = Vector3.MoveTowards(transform.position, frontPosition, speed * 0.1f);
        }

        else if (nextMove <= -1)
        {
            transform.position = Vector3.MoveTowards(transform.position, rememberPosition, speed * 0.1f);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawRay(transform.position, transform.right * range);
        
    }

}