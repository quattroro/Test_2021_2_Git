using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private bool invectoryActivated = true;  // �κ��丮 Ȱ��ȭ ����. true�� �Ǹ� ī�޶� �����Ӱ� �ٸ� �Է��� ���� ���̴�.

    [SerializeField]
    private GameObject go_InventoryBase; // Inventory_Base �̹���
    [SerializeField]
    private GameObject go_SlotsParent;  // Slot���� �θ��� Grid Setting 

    private slot[] slots;  // ���Ե� �迭

    void Start()
    {
        slots = go_SlotsParent.GetComponentsInChildren<slot>();
        //go_InventoryBase.SetActive(invectoryActivated);
    }

    void Update()
    {
        TryOpenInventory();
    }

    private void TryOpenInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            invectoryActivated = !invectoryActivated;

            //if (invectoryActivated)
            //    go_InventoryBase.SetActive(invectoryActivated);
            ////OpenInventory();
            //else
            //    go_InventoryBase.SetActive(invectoryActivated);
            //    //CloseInventory();

        }
        go_InventoryBase.SetActive(invectoryActivated);
    }

    private void OpenInventory()
    {
        go_InventoryBase.SetActive(true);
    }

    private void CloseInventory()
    {
        go_InventoryBase.SetActive(false);
    }

    public void AcquireItem(Item _item, int _count = 1)
    {
        if (Item.ItemType.Equipment != _item.itemType)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item != null)  // null �̶�� slots[i].item.itemName �� �� ��Ÿ�� ���� ����
                {
                    if (slots[i].item.itemName == _item.itemName)
                    {
                        slots[i].SetSlotCount(_count);
                        return;
                    }
                }
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].AddItem(_item, _count);
                return;
            }
        }
    }
}