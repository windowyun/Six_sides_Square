using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField]
    GameObject camera1;

    [SerializeField]
    Transform startPoint;
    [SerializeField]
    Transform endPoint;

    bool useCoin = false;

    public float moveSpeed = 2.0f;

    float hor = 0f;
    float vel = 0f;

    RaycastHit hitlimit;
    RaycastHit hitUpG;
    RaycastHit hitZ;
    RaycastHit hitX;

    Rigidbody rigid;

    bool spaceOn = false;
    public bool SpaceOn
    {
        get { return spaceOn; }
    }
    float spaceTime = 0f;
    float spaceAngle = 0f;
    Vector3 transUp;

    Vector3 spawnPoint = Vector3.zero;
    Quaternion savePlayer = Quaternion.identity;

    int currentCoin = 0;
    List<GameObject> coins = new List<GameObject>();

    GameObject[] coinCount;
    GameObject[] enemyCount;

    Vector3[] coinPosition;
    Vector3[] enemyPosition;


    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }
    void Start()
    {
        coinCount = GameObject.FindGameObjectsWithTag("Coin");
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy");

        coinPosition = new Vector3[coinCount.Length];
        enemyPosition = new Vector3[enemyCount.Length];

        if(coinCount != null)
        {
            useCoin = true;
            currentCoin = coinCount.Length;
        }

        StartCoroutine(RemeberPosition());

        Vector3 UpY = transform.up * (startPoint.localScale.y - 0.01f);

        spawnPoint = startPoint.position + UpY;
        savePlayer *= transform.rotation;

        transform.position = spawnPoint;

    }

    void FixedUpdate()
    {
        hor = Input.GetAxisRaw("Horizontal");
        vel = Input.GetAxisRaw("Vertical");


        bool raycastG = Physics.Raycast(transform.position + (transform.forward * vel + transform.right * hor) * 0.54f, transform.up * -1, out hitlimit, Mathf.Infinity, LayerMask.GetMask("Ground"));
        Physics.Raycast(transform.position, transform.forward * vel, out hitZ, 1f, LayerMask.GetMask("Ground"));
        Physics.Raycast(transform.position, transform.right * hor, out hitX, 1f, LayerMask.GetMask("Ground"));


        if (raycastG)
        {   
            rigid.velocity = (transform.forward * vel + transform.right * hor).normalized * moveSpeed;
        }

        else
            rigid.velocity = Vector3.zero;

        if (spaceOn)
        {
            transform.Translate(transUp * 0.3f, Space.World);

            Transform angletrans = transform;

            //transform.rotation = Quaternion.Slerp(transform.rotation, hitUpG.transform.rotation, spaceDistance );
            //transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, 0f, spaceAngle));
            
        }


    }

    void Update()
    {
      
        if (!spaceOn && Input.GetKeyDown(KeyCode.Space) && Physics.Raycast(transform.position, transform.up, out hitUpG, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            spaceOn = true;
            transUp = Vector3.zero;
            transUp += transform.up;

            

            spaceTime = hitUpG.distance - 0.5f;
            rigid.velocity = Vector3.zero;
            spaceTime = spaceTime / 10.0f;
            float angle = hitUpG.transform.rotation.eulerAngles.z - transform.rotation.eulerAngles.z;
            spaceAngle = (1f / spaceTime) * angle;
            spaceAngle /= 50f;
            // = hitUpG.transform.rotation.z / spaceDirect;

        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            if (spaceOn)
            {
                if (collision.transform == hitUpG.transform)
                {
                    spaceOn = false;
                    transform.rotation *= Quaternion.Euler(0f,0f, 180f);
                    camera1.GetComponent<CameraMove>().RotSame = false;
                }
            }

            else if(!spaceOn)
            {
                if (hitZ.transform == collision.transform)
                {
                    //transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles) * addAngle;

                    transform.rotation = Quaternion.FromToRotation(transform.forward * vel, transform.up) * Quaternion.Euler(transform.rotation.eulerAngles);

                    camera1.GetComponent<CameraMove>().RotSame = false;

                }

                if (hitX.transform == collision.transform)
                {
                    //transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles) * addAngle;

                    transform.rotation = Quaternion.FromToRotation(transform.right * hor, transform.up) * Quaternion.Euler(transform.rotation.eulerAngles);
 
                    camera1.GetComponent<CameraMove>().RotSame = false;

                }
            }
        }
    }

    

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            spaceOn = false;
            transform.rotation = savePlayer;
            transform.position = spawnPoint;
            camera1.GetComponent<CameraMove>().RotSame = false;
            StartCoroutine(ActiveCoin());
        }

        else if (other.gameObject.tag == "Coin")
        {
            other.gameObject.SetActive(false);
            coins.Add(other.gameObject);
        }

        else if (other.gameObject.tag == "Point")
        {
            if (useCoin)
            {
                if (currentCoin - coins.Count <= 0 && endPoint == other.transform)
                {
                    SceneManager.LoadScene(1);
                }

                else
                {
                    currentCoin -= coins.Count;
                    coins.Clear();

                    Spawn(other);
                }
            }

            else
            {
                if(other.transform == endPoint)
                {

                }

                else
                {
                    Spawn(other);
                }
            }
        }

        

    }

    void Spawn(Collider other)
    {
        if (spaceOn)
        {
            Vector3 UpY = transform.up * (other.transform.localScale.y - 0.01f) * -1f;

            savePlayer = transform.rotation * Quaternion.Euler(0f, 0f, 180f);
            spawnPoint = other.transform.position + UpY;
        }

        else
        {
            Vector3 UpY = transform.up * (other.transform.localScale.y - 0.01f);

            spawnPoint = other.transform.position + UpY;
            savePlayer = transform.rotation;
        }
    }

    IEnumerator RemeberPosition()
    {
        if (coinCount != null)
        {
            for (int i = 0; i < coinCount.Length; i++)
            {
                coinPosition[i] = coinCount[i].transform.position;
            }
        }

        if (enemyCount != null)
        {
            for (int i = 0; i < enemyCount.Length; i++)
            {
                enemyPosition[i] = enemyCount[i].transform.position;
            }
        }
        yield return null;
    }

    IEnumerator ActiveCoin()
    {
        for (int i = 0; i < coins.Count; i++)
        {
            coins[i].SetActive(true);
        }

        coins.Clear();

        yield return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        bool raycastG = Physics.Raycast(transform.position + (transform.forward * v + transform.right * h) * 0.55f, transform.up * -1, out hitlimit, Mathf.Infinity, LayerMask.GetMask("Ground"));
        if (raycastG)
            Gizmos.DrawRay(transform.position + (transform.forward * v + transform.right * h) * 0.55f, transform.up * hitlimit.distance * -1f);
        else
            Gizmos.DrawRay(transform.position + (transform.forward * v + transform.right * h) * 0.55f, transform.up * -10f);

        bool rayZ = Physics.Raycast(transform.position, transform.forward * v, out hitZ, 1f, LayerMask.GetMask("Ground"));
        if (rayZ)
            Gizmos.DrawRay(transform.position, transform.forward * v * hitZ.distance);
        else
            Gizmos.DrawRay(transform.position, transform.forward * v);

        bool rayX = Physics.Raycast(transform.position, transform.right * h, out hitX, 1f, LayerMask.GetMask("Ground"));
        if (rayX)
            Gizmos.DrawRay(transform.position, transform.right * h * hitX.distance);
        else
            Gizmos.DrawRay(transform.position, transform.right * h);

        
    }
}
