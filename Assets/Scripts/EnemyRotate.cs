using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRotate : MonoBehaviour
{
    [SerializeField]
    float spinNum = 1f;
    //[SerializeField]
    //float speed = 1f;

    Rigidbody rigid;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        transform.rotation *= Quaternion.Euler(Vector3.up * (spinNum * 360 / 50));

        //rigid.velocity = transform.right * speed;
    }
}
