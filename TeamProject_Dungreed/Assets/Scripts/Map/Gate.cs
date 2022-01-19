using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public enum GateType { START, END };

    //public LayerMask PlayerLayer;

    public GateType type;
    public PlayerInteraction interaction;

    public void CheckPlayer()
    {

    }
    public void StageClear()
    {
        Debug.Log("StageClear!");
    }

    // Start is called before the first frame update
    void Start()
    {
        if(type==GateType.END)
        {
            interaction = GetComponentInChildren<PlayerInteraction>();
            interaction.AddAction(StageClear);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
