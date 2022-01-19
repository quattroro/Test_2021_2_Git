using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragSlot : MonoBehaviour
{
    static public DragSlot Instance = null;
    public slot slot;
   public slot tempSlot =null;

    [SerializeField]
    private Image imageItem;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void ResetImage()
    {
        imageItem = null;
    }
    public void DragSetImage(Image _itemImage)
    {
        imageItem = _itemImage;
        imageItem.sprite = _itemImage.sprite;
    
        SetColor(1);
        this.GetComponent<Image>().sprite=imageItem.sprite;
        this.GetComponent<Image>().SetNativeSize();

    }

    public void SetColor(float _alpha)
    {
        Color color = this.GetComponent<Image>().color;
        color.a = _alpha;
        this.GetComponent<Image>().color=color;

    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
