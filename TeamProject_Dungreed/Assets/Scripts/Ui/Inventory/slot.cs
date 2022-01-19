using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class slot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,IDropHandler,IPointerClickHandler
{
  
    private Text text_Count;

    public Sprite nullitem=null;
    public Image MiniItem = null;
    public Item item=null;
    public int itemCount;
    public Image itemImage;
    public GameObject target;
    private bool able = true;

    // Start is called before the first frame update
    void Start()
    {
        itemImage = GetComponent<Image>();
       
        if (item != null)
        {
            AddItem(item);
        }
    }
    private void SetColor(float _alpha)
    {
        Color color = MiniItem.color;
        color.a = _alpha;
        MiniItem.color = color;
        //this.GetComponent<Image>().color = color;
    }
    public void AddItem(Item _item, int _count = 1)
    {

        if (able)
        {
            item = _item;
            itemCount = _count;

            Color color = MiniItem.color;
            color.a = 1;
            MiniItem.color = color;

            MiniItem.sprite = item.itemImage;
            MiniItem.SetNativeSize();
        }

        //  itemImage.sprite = item.itemImage;
        //  MiniItem.sprite
        // SetColor(1);
        //   this.GetComponent<Image>().sprite = itemImage.sprite;
    }

    public void SetSlotCount(int _count)
    {
        itemCount += _count;
      //  text_Count.text = itemCount.ToString();

        if (itemCount <= 0)
            ClearSlot();
    }

    private void ClearSlot()
    {
        item = null;
        itemCount = 0;
       // itemImage.sprite = null;
       // SetColor(0);
        //this.GetComponent<Image>().sprite = nullitem;// itemImage.sprite;

        this.MiniItem.color=new Color(255,255,255,0);
        //text_Count.text = "0";

    }

    // Update is called once per frame
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log(this.name);
        if (item != null)
        {
            DragSlot.Instance.slot = this;
            DragSlot.Instance.DragSetImage(MiniItem);
            DragSlot.Instance.transform.position = eventData.position;
            DragSlot.Instance.tempSlot = this;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            DragSlot.Instance.transform.position = eventData.position;
           
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (DragSlot.Instance.slot != null)
        {
            DragSlot.Instance.SetColor(0);
            DragSlot.Instance.slot = null;
        }

        if(!eventData.pointerCurrentRaycast.gameObject)
        {
            DragSlot.Instance.tempSlot.SetColor(1);
            Debug.Log("½½·Ô¾Æ´Ñ¤±");
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        //Item tempitem=this.item;
        //Image tempitemImage = this.itemImage;
        //this.item = DragSlot.Instance.slot.item;
        //this.itemImage.sprite = DragSlot.Instance.slot.itemImage.sprite;
        //DragSlot.Instance.slot.item = tempitem;
        //DragSlot.Instance.slot.itemImage.sprite = tempitemImage.sprite;
        Debug.Log("sdgssgs");
        if(DragSlot.Instance.slot!=null)
        {
            ChangeSlot();
         //   this.SetColor(1);
        }
        
    }

    private void ChangeSlot()
    {
        Item _tempItem = item;
        int _tempItemCount = itemCount;

        AddItem(DragSlot.Instance.slot.item, DragSlot.Instance.slot.itemCount);

        if(_tempItem!=null)
        {
            DragSlot.Instance.slot.AddItem(_tempItem, _tempItemCount);
            DragSlot.Instance.slot.SetColor(1);        }
        else
        {
            DragSlot.Instance.slot.ClearSlot();
        }

        DragSlot.Instance.slot = null;
        DragSlot.Instance.ResetImage();
        DragSlot.Instance.SetColor(0);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(this.name);
        DragSlot.Instance.slot = this;
    }


    // Update is called once per frame
    void Update()
    {
        //if (Input.GetMouseButton(0))
        //{
        //    Vector3 mos = Input.mousePosition;
        //    mos.z = Camera.main.farClipPlane;

        //    Vector3 dir = Camera.main.ScreenToWorldPoint(mos);

        //    RaycastHit hit;
        //    if (Physics.Raycast(transform.position, dir, out hit, mos.z))
        //    {
        //        target.transform.position = hit.point;
        //    }
        //}
    }

   
}
