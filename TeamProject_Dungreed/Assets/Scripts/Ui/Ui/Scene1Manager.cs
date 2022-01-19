using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene1Manager : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject option_Panel;
    public GameObject bird;
    public bool stopSpawn = true;
    void Start()
    {
        StartCoroutine(SpawnBird());
    }

    IEnumerator SpawnBird()
    {
        while (stopSpawn)
        {
           yield return new WaitForSecondsRealtime(10f);
           Instantiate(bird);
        }
    }
    public void optionPanelOn()
    {
        option_Panel.transform.gameObject.SetActive(true);
    }
    public void optionPanelOff()
    {
        option_Panel.transform.gameObject.SetActive(false);
    }
    public void SceneChange()
    {
        SceneManager.LoadScene("gameScene");
    }
    public void stopSpawning()
    {
        stopSpawn = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
