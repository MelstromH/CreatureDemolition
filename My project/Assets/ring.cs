using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ring : MonoBehaviour
{
    public float rate = 0.02f;
    private Vector3 scaleChange;
    // Start is called before the first frame update
    void Start()
    {
        scaleChange = new Vector3(rate, 0, rate);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //affect change in size of platform
    
        gameObject.transform.localScale += scaleChange;
    }
}
