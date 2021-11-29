using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField]
    Player player;
    [SerializeField]
    Transform rotatePlayer;
    [SerializeField]
    Transform cameraAxis;
    [SerializeField]
    Transform cameraArm;
    [SerializeField]
    float speed = 10f;
    
    [SerializeField]
    bool isStatic = false;
    [SerializeField]
    bool isFollowAngle = false;
    [SerializeField]
    bool isFollow = false;
    [SerializeField]
    bool isLookAround = false;
    [SerializeField]
    bool isZoomin = false;
    [SerializeField]
    bool isWallTranslucent = false;


    bool rotSame = true;
    public bool RotSame
    {
        set { rotSame = value; }
    }

    float sameOrder = 0f;

    //[SerializeField]
    //float mixRange = 1f;
    //[SerializeField]
    //float maxRange = 10f;

    Renderer obstaclesRenderer;

    Camera thiscamera;

    Vector3 worldDefalut;

    RaycastHit hit;

    void Start()
    {
        thiscamera = GetComponent<Camera>();
        //worldDefalut = transform.forward;

        if (isFollow)
        {
            cameraAxis.position = rotatePlayer.position;
            cameraAxis.rotation = rotatePlayer.rotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        if (!isStatic)
        {
            LookAround();
            zoomin();
            followRotate();
            wallTranslucent();

            if (isFollow)
                cameraAxis.position = rotatePlayer.position;
        }
        //if(Input.GetKeyDown(KeyCode.J))
        //{
        //    Debug.Log("camera : " + cameraArm.rotation.eulerAngles);
        //    Debug.Log("Player : " + rotatePlayer.rotation.eulerAngles);
        //}
    }

    void LookAround()
    {
        if (isLookAround &&Input.GetKey(KeyCode.Mouse0) && (!isFollowAngle || rotSame))
        {
            Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * 2f;

            Vector3 camAngle = cameraArm.localRotation.eulerAngles;

            //Vector3 x = rotatePlayer.right * -mouseDelta.y;
            //Vector3 y = rotatePlayer.up * mouseDelta.x;

            //cameraArm.rotation = Quaternion.Euler(camAngle.x - mouseDelta.y, camAngle.y + mouseDelta.x, camAngle.z);
            cameraArm.localRotation = Quaternion.Euler(camAngle.x - mouseDelta.y, camAngle.y + mouseDelta.x, camAngle.z);

        }
    }
    

    void zoomin()
    {
        if (isZoomin)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel") * speed;

            //Vector3 Targetpos = cameraArm.position - transform.position;
            //Targetpos = Targetpos.normalized;
            //transform.position = transform.position + (Targetpos * scroll);

            transform.position += transform.forward * scroll;
        }
    }

    void followRotate()
    {
        if(isFollowAngle)
        {
            if (player.SpaceOn)
            {
                cameraAxis.position = rotatePlayer.position;
            }

            else
            {
                if(cameraAxis.rotation == rotatePlayer.rotation || sameOrder > 1f)
                {
                    cameraAxis.rotation = rotatePlayer.rotation;
                    sameOrder = 0f;
                    rotSame = true;
                }

                else if (!rotSame)
                {

                    sameOrder += Time.deltaTime;

                    cameraAxis.position = rotatePlayer.position;
                    cameraAxis.rotation = Quaternion.Slerp(cameraAxis.rotation, rotatePlayer.rotation, Time.deltaTime * 10);
                    //wallPass();
                    
                }
            } 
        }
    }
    /*
    void wallPass()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, (rotatePlayer.position - transform.position).magnitude - 0.5f, LayerMask.GetMask("Ground")))
        {
            transform.position += transform.forward * Mathf.Lerp(0f, hit.distance - 0.5f, 0.5f);
        }
    }
    */

    void wallTranslucent()
    {

        if (isWallTranslucent)
        { 
            if (Physics.Raycast(transform.position, transform.forward, out hit, (rotatePlayer.position - transform.position).magnitude - 0.5f, LayerMask.GetMask("Ground")))
            {
                if (obstaclesRenderer != hit.collider.gameObject.GetComponent<Renderer>())
                {

                    if (obstaclesRenderer != null)
                    {
                        Material matin = obstaclesRenderer.material;

                        Color matColorin = matin.color;
                        matColorin.a = 1f;
                        matin.color = matColorin;

                        obstaclesRenderer = null;
                    }

                    obstaclesRenderer = hit.collider.gameObject.GetComponent<Renderer>();

                    Material mat = obstaclesRenderer.material;

                    Color matColor = mat.color;
                    matColor.a = 0.5f;
                    mat.color = matColor;
                }
            }

            else
            {
                if (obstaclesRenderer != null)
                {
                    Material mat = obstaclesRenderer.material;

                    Color matColor = mat.color;
                    matColor.a = 1f;
                    mat.color = matColor;

                    obstaclesRenderer = null;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawRay(transform.position, transform.forward * hit.distance);
    }
}
