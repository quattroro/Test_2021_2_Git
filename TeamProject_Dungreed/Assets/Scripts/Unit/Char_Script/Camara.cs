using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camara : MonoBehaviour
{
    public Camara camara;
    public GameObject obj_player;
  
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 a = camara.transform.position;
        a = obj_player.transform.position;
        a.z = -10;
        a.y = obj_player.transform.position.y;
        camara.transform.position = a;
    }
}
