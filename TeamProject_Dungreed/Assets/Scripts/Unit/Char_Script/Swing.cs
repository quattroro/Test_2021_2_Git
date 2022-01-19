using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swing : MonoBehaviour
{
    [SerializeField]
    private Vector3 tmp;
    public GameObject obj_swing;
    private GameObject obj_copy;
    private float rotateDegree;
    private Vector2 localscale;
    // Start is called before the first frame update
    void Start()
    {
        localscale = this.transform.localScale;
    }
    // Update is called once per frame
    void Update()
    {
        //SwingRotation();
    }
    void SwingRotation()
    {
        tmp = this.transform.localPosition;
        tmp.y = 0.12f;

        obj_copy = Instantiate(obj_swing);
        obj_copy.transform.parent = this.transform;
        obj_copy.transform.localRotation = Quaternion.Euler(0, 0, 7.377f);
        obj_copy.transform.localPosition = tmp;
        this.transform.localScale = localscale;
       
        Vector3 mPosition = Input.mousePosition; //마우스 좌표 저장
        Vector3 oPosition = transform.position; //게임 오브젝트 좌표 저장
        mPosition.z = oPosition.z - Camera.main.transform.position.z;
        Vector3 target = Camera.main.ScreenToWorldPoint(mPosition);
        float dy = target.y - oPosition.y;
        float dx = target.x - oPosition.x;
        rotateDegree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.Euler(0f, 0f, rotateDegree - 90);

        
    }

    public void SwingEffect()
    {
        SwingRotation();
        Destroy(obj_copy, 0.2f);
    }
  
}
