using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 temp = this.transform.position;
        
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        temp = new Vector3(h, v);

        this.transform.position += temp * 20 * Time.deltaTime;

    }
}
