using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{

    public LayerMask PlayerMask;

    public float ExplosivePower;

    public float knockbacktime;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Ãæµ¹ µé¾î¿È1");
        if(collision.gameObject.layer==LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Ãæµ¹ µé¾î¿È1");
            Vector3 direction = collision.transform.position - this.transform.position;
            direction.Normalize();
            collision.GetComponent<Character2DMove>().KnockBack(direction * ExplosivePower, knockbacktime);
        }
        
    }
    
    public void Explosive()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
