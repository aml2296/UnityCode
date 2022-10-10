using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[Serializable]
public class InventorySlot
{
    [SerializeField]
    public GameObject main = null;
    [SerializeField]
    public GameObject meshObj = null;
    [SerializeField]
    MeshRenderer mr = null;
    [SerializeField]
    MeshFilter mf = null;
    [SerializeField]
    public int count = 0;
    public InventorySlot() { }
    public void setupMesh(Mesh m, Material mat)
    {
        if (main == null)
            throw new System.Exception("No Main SetUp");
        if (meshObj == null)
            meshObj = new GameObject("Mesh");
        if (mf == null)
            mf = meshObj.AddComponent<MeshFilter>();
        if(mr == null)
            mr = meshObj.AddComponent<MeshRenderer>();


        meshObj.layer = LayerMask.NameToLayer("Overlay");
        meshObj.transform.SetParent(main.transform);
        meshObj.transform.localScale = Vector3.one * 5f;
        meshObj.transform.localPosition = new Vector3(9, -8, -6);
        meshObj.transform.localRotation = Quaternion.Euler(0, 0, 75);
        Rotation rotate = meshObj.AddComponent<Rotation>();
        rotate.FullRotationPerMinute = Vector3.right * 60;
        mf.mesh = m;
        mr.material = mat;
        if (main.activeSelf && meshObj.activeInHierarchy)
            rotate.BeginRotate();
    }
    public void updateCount()
    {
        if (main)
            main.GetComponentInChildren<TextMeshProUGUI>().SetText(count.ToString());
    }
}

[Serializable]
public class Inventory : MonoBehaviour
{
    public event Action<Item> addItem;
    public event Action<Item, int> removeItem;

    [SerializeField]
    GameObject prefab;
    [SerializeField]
    GameObject parent;

    Dictionary<int, InventorySlot> inventory = new Dictionary<int, InventorySlot>();

    public void Start()
    {
        addItem += AddToInv;
        removeItem += RemoveFromInv;
    }
    public void OnDestroy()
    { 
        addItem -= AddToInv;
        removeItem -= RemoveFromInv;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            Item item = other.gameObject.GetComponent<Item>();
            if (item)
            {
                addItem?.Invoke(item);
                item.InvokePickUp();
            } 
        }
    }

    public void AddToInv(Item item)
    {
        InventorySlot invSlot = new InventorySlot();
        int id = item.getID();
        int value = item.getValue();
        try
        {
            if (inventory.ContainsKey(id))
            {
                invSlot = inventory[id];
                invSlot.count += value;
                invSlot.updateCount();
                inventory.Add(id, invSlot);
                return;
            }
            invSlot.main = Instantiate(prefab, parent.transform);
            invSlot.main.transform.localPosition += Vector3.right*inventory.Count * invSlot.main.GetComponent<RectTransform>().rect.width;
            invSlot.setupMesh(item.GetComponentInChildren<MeshFilter>().mesh, item.GetComponentInChildren<MeshRenderer>().material);
            invSlot.count = value;
            invSlot.updateCount();
            inventory.Add(id, invSlot);
        }
        catch(ArgumentException e)
        {
            inventory.Remove(id);
            inventory.Add(id, invSlot);
        }
    }
    public void RemoveFromInv(Item item, int value)
    {
        int id = item.getID();
        if(inventory.ContainsKey(id))
        {
            InventorySlot invSlot = inventory[id];
            invSlot.count -= value;
            if (invSlot.count <= 0)
                inventory.Remove(id);
                
        }
    }
    public int Count(int id)
    {
        return inventory.ContainsKey(id) ? inventory[id].count : 0;
    }
}
