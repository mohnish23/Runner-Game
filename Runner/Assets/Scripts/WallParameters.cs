using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WallParameters : MonoBehaviour
{
    public int WallValue;
    [Range(0f,1f)]
    public float wallAlpha;
    private int initialVal;
    public TextMeshProUGUI valTxt;

    // Start is called before the first frame update
    void Start()
    {
        initialVal = WallValue;
    }

    // Update is called once per frame
    void Update()
    {
        valTxt.text = WallValue.ToString();

        GetComponent<Renderer>().material.color = new Color(1, 0.482f, 0.482f, wallAlpha);
        wallAlpha = Mathf.Lerp(wallAlpha, WallValue / initialVal, Time.deltaTime * 1f);

        if(WallValue <= 0)
        {
            Destroy(gameObject);
        }
    }
}
