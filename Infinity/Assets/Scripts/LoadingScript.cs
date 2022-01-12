using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScript : MonoBehaviour
{
    public float speed;
    public Slider slider;
    public float count;

    public float LoadingScond;

    public float starttime;
    public float stoptime;
    // Start is called before the first frame update
    void Start()
    {
        starttime = Time.time;
    }

    


    // Update is called once per frame
    void Update()
    {
        slider.value += Time.deltaTime * speed;
        if (slider.value>=100)
        {
            SceneManager.LoadScene(2);
            return;
        }
        
        
    }
}
