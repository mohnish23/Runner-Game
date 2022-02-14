using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerParent : MonoBehaviour
{
    [Header("GameObject name has to be PlayerParent \n[referenced by player script start method]\n\n")]
    public List<Player> playerArray = new List<Player>();
    [SerializeField]
    private float posSum;
    [SerializeField]
    private float touchSpeed;
    [SerializeField]
    private bool move = true;
    private Rigidbody rb;
    public float speed = 15f;
    public Transform[] CharPositions;
    public Vector3 Offset;
    public float camValX, camValY, camValZ;
    private Vector2 StartPos;
    public Player MergeObj1;
    public Player MergeObj2;
    public bool canMerge = true;
    public bool canSplit = false;
    public int WallVal;
    public GameObject WallObj;
    public bool snapToPos = true;
    public GameObject GameOverMenu;
    public GameObject LevelCompleteMenu;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        rb = GetComponent<Rigidbody>();
        Offset.x = camValX;
        Offset.y = camValY;
        Offset.z = camValZ;
        StartCoroutine(Merge());
    }

    private void FixedUpdate()
    {
        if (move == true)
        {
            rb.velocity = new Vector3(0, 0, speed);

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                this.transform.position = new Vector3(transform.position.x + touch.deltaPosition.x * touchSpeed, transform.position.y, transform.position.z);

                if (touch.phase == TouchPhase.Began)
                {
                    StartPos = touch.position;
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    if (touch.position.x > StartPos.x)
                    {
                        for (int i = 0; i < playerArray.Count; i++)
                        {
                            playerArray[i].RotY = 35f;
                            StartCoroutine(RotationReset());
                        }
                    }
                    else
                    {
                        for (int i = 0; i < playerArray.Count; i++)
                        {
                            playerArray[i].RotY = -35f;
                            StartCoroutine(RotationReset());
                        }
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        CamController();
        StartCoroutine(DeathCheck());

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -3.8f, 3.8f), transform.position.y, transform.position.z);

        Player[] p = GetComponentsInChildren<Player>();

        for(int i = 0; i < p.Length; i++)
        {
            playerArray.Clear();

            for(int x = 0; x < p.Length; x++)
            {
                playerArray.Add(p[x]);
            }
        }

        for(int j = 0; j < playerArray.Count; j++)
        {
            playerArray = playerArray.OrderByDescending(x => x.PlayerValue).ToList();

            if(snapToPos == true)
            {
                if (j < CharPositions.Length)
                {
                    var pos = CharPositions[j].position;
                    playerArray[j].CharPosition = new Vector3(pos.x, playerArray[j].transform.position.y, pos.z);
                }
                else
                {
                    var cPos = CharPositions[CharPositions.Length - 1].position;
                    playerArray[j].CharPosition = new Vector3(cPos.x - 2, playerArray[j].transform.position.y, cPos.z - 2);
                }
            }
        }
    }

    public IEnumerator DeathCheck()
    {
        yield return new WaitForSeconds(0.1f);
        if(playerArray[0] == null)
        {
            GameOverMenu.SetActive(true);
            Destroy(gameObject);
            StopAllCoroutines();
        }
    }

    public void CamController()
    {
        Offset.x = Mathf.Lerp(Offset.x, camValX, Time.deltaTime * 2f);
        Offset.y = Mathf.Lerp(Offset.y, camValY, Time.deltaTime * 2f);
        Offset.z = Mathf.Lerp(Offset.z, camValZ, Time.deltaTime * 2f);

        Camera.main.transform.position = new Vector3(Offset.x, transform.position.y + Offset.y, transform.position.z + Offset.z);
    }

    public IEnumerator RotationReset()
    {
        yield return new WaitForSeconds(0.3f);
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            StartPos = touch.position;
        }

        for (int i = 0; i < playerArray.Count; i++)
        {
            playerArray[i].RotY = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Wall")
        {
            speed = 0;
            WallObj = other.gameObject;
            StartCoroutine(Split());
        }

        if(other.tag == "Finish")
        {
            LevelCompleteMenu.SetActive(true);
            speed = 0;
        }
    }

    public IEnumerator Merge()
    {
        while (canMerge == true)
        {
            yield return new WaitForSeconds(0.1f);

            for (int i = 0; i < playerArray.Count; i++)
            {
                if(MergeObj2 == null)
                {
                    if (playerArray[i].PlayerValue != 100)
                    {
                        MergeObj1 = playerArray[i];
                    }

                    foreach (Player j in playerArray)
                    {
                        if (j != MergeObj1 && MergeObj1 != null && j.PlayerValue == MergeObj1.PlayerValue)
                        {
                            MergeObj2 = j;
                        }
                    }
                }
            }

            if (MergeObj1 != null && MergeObj2 != null)
            {
                var Upgrade = MergeObj1.Upgrade;
                var instantiatePos = MergeObj1.transform.position;
                yield return new WaitForSeconds(0.35f);
                if(MergeObj2 != null)
                {
                    MergeObj2.transform.SetParent(null);
                    MergeObj2.gameObject.GetComponent<Collider>().isTrigger = true;
                    MergeObj2.CharPosition = MergeObj1.transform.position;
                }
                yield return new WaitForSeconds(0.2f);
                if (MergeObj1 != null)
                    Destroy(MergeObj1.gameObject);
                if (MergeObj2 != null)
                    Destroy(MergeObj2.gameObject);
                MergeObj1 = null;
                MergeObj2 = null;
                var g = Instantiate(Upgrade, instantiatePos, Quaternion.identity, transform);
                g.GetComponent<Player>().Laying = false;
                g.GetComponent<Player>().isGrouped = true;
            }
        }
    }

    public IEnumerator Split()
    {
        bool loop = true;
        while(loop == true)
        {
            yield return new WaitForSeconds(0.09f);

            move = false;
            snapToPos = false;
            canMerge = false;

            for (int i = 0; i < playerArray.Count; i++)
            {
                if (playerArray[i] == null)
                {
                    GameOverMenu.SetActive(true);
                    Destroy(gameObject);
                    StopAllCoroutines();
                }
                else
                {
                    playerArray[i].GetComponent<Animator>().SetBool("Pushing", true);
                }

                //playerArray[i].CharPosition = new Vector3(playerArray[i].CharPosition.x, playerArray[i].CharPosition.y, playerArray[0].CharPosition.z);
            }

            if(WallObj != null)
            {
                WallObj.GetComponent<WallParameters>().WallValue -= 1;
            }

            var lastElement = playerArray[playerArray.Count - 1];

            if (lastElement.PlayerValue <= 25 && lastElement.PlayerValue > 0)
            {
                for(int i = 0; i < lastElement.PlayerValue; i++)
                {
                    if(WallObj != null && WallObj.GetComponent<WallParameters>().WallValue > 0)
                    {
                        WallObj.GetComponent<WallParameters>().WallValue -= 1;
                        lastElement.PlayerValue -= 1;
                    }
                    else
                    {
                        for (int j = 0; j < playerArray.Count; j++)
                        {
                            playerArray[j].GetComponent<Animator>().SetBool("Pushing", false);
                        }
                        snapToPos = true;
                        canMerge = true;
                        move = true;
                        speed = 15f;
                        loop = false;
                        StartCoroutine(Merge());
                        break;
                    }
                }
            }
            else if (lastElement.PlayerValue > 25)
            {
                var instPos = lastElement.transform.position;
                yield return new WaitForSeconds(0.1f);
                var a = Instantiate(lastElement.Downgrade, new Vector3(instPos.x, instPos.y, instPos.z - 6), Quaternion.identity, transform);
                var b = Instantiate(lastElement.Downgrade, new Vector3(instPos.x, instPos.y, instPos.z - 6), Quaternion.identity, transform);
                a.GetComponent<Player>().CharPosition = new Vector3(instPos.x + 0.8f, instPos.y, instPos.z);
                b.GetComponent<Player>().CharPosition = new Vector3(instPos.x - 0.8f, instPos.y, instPos.z);
                a.GetComponent<Player>().isGrouped = true;
                b.GetComponent<Player>().isGrouped = true;
                a.GetComponent<Player>().Laying = false;
                b.GetComponent<Player>().Laying = false;
                a.GetComponent<Animator>().SetBool("Pushing", true);
                b.GetComponent<Animator>().SetBool("Pushing", true);
                if (lastElement != null)
                {
                    Destroy(lastElement.gameObject);
                }
            }
            else
            {
                if (lastElement != null)
                {
                    Destroy(lastElement.gameObject);
                    loop = false;
                    StopCoroutine(Split());
                    break;
                }
            }
        }
    }
}
