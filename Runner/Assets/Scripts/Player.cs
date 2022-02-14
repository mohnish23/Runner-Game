using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int PlayerValue;
    public Text PlayerText;

    public Vector3 CharRotation;
    public Vector3 CharPosition;
    public bool isGrouped = true;
    public float RotY;

    public float PositionWhileLaying;
    public float PositionWhileRunning;
    public bool Laying = false;

    public GameObject Upgrade;
    public GameObject Downgrade;
    public GameObject PlayerOne;

    public GameObject WallObj;
    public Transform playerParent;

    // Start is called before the first frame update
    void Start()
    {
        playerParent = GameObject.Find("PlayerParent").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerText.text = PlayerValue.ToString();

        RotationController();
        if(isGrouped == true)
        {
            PositionController();
        }

        if(Laying == true)
        {
            GetComponent<Animator>().SetBool("Running", false);
            transform.position = new Vector3(transform.position.x, PositionWhileLaying, transform.position.z);
        }
        else
        {
            GetComponent<Animator>().SetBool("Running", true);
            transform.position = new Vector3(transform.position.x, PositionWhileRunning, transform.position.z);
        }

        if(PlayerValue <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void RotationController()
    {
        CharRotation.y = Mathf.Lerp(CharRotation.y, RotY, Time.deltaTime * 8f);
        transform.eulerAngles = CharRotation;
    }

    public void PositionController()
    {
        transform.position = Vector3.Lerp(transform.position, CharPosition, Time.deltaTime * 12f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.transform.SetParent(playerParent);
            collision.gameObject.GetComponent<Player>().Laying = false;
            collision.gameObject.GetComponent<Player>().isGrouped = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            PlayerValue -= 1;
            Instantiate(PlayerOne, new Vector3(transform.position.x + Random.Range(-2,2), transform.position.y, transform.position.z - 1), Quaternion.identity);
        }
    }
}
