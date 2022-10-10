using System;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public event Action<Player> BuyEvent;
    [SerializeField]
    float power = 1f;
    [SerializeField]
    Item costObj = null;
    [SerializeField]
    int costValue = 1;
    [SerializeField]
    GameObject output = null;
    
    public bool CanBuy(int itemID, Inventory inv)
    {
        Item itemCost = costObj.GetComponent<Item>();
        if (itemCost is Item)
        {
            if (costValue <= inv.Count(itemCost.getID()))
            {
                return true;
            }
        }
        return false;
    }
    public bool Buy(int itemID, Player player)
    {
        bool canBuyItem = CanBuy(itemID, player.getInventory());
        if (canBuyItem)
            BuyEvent?.Invoke(player);
        return canBuyItem;
    }
    private void Pay(Player player)
    {
        try
        {
            Inventory inv = player.getInventory();
            if (costObj == null)
                throw new System.Exception("No Item attatched!");

            inv.RemoveFromInv(costObj, costValue);
        }
        catch (Exception e){ Debug.LogException(e); }
    }
    private void Sell(Player player)
    {
        GameObject obj = Instantiate(output, transform.position + (transform.forward * 2), Quaternion.identity);

        Rigidbody rbody = obj.GetComponent<Rigidbody>();
        if (rbody)
        {
            Debug.Log("Moved Rbody!");
            rbody.AddForce(Vector3.forward * power + Vector3.up * power / 2);
        }


        CubeEntity cubie = obj.GetComponent<CubeEntity>();
        if(cubie)
        {
            cubie.setTarget(player.gameObject.transform);
        }

    }
    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Shop"))
        {
            Player player = other.GetComponentInParent<Player>();
            if (player)
            {
                if (Buy(costObj.getID(), player))
                {
                    Debug.Log("SOLD");
                }
            }
        }
    }
    public void Start()
    {
        BuyEvent += Sell;
        BuyEvent += Pay;
    }
    public void OnDestroy()
    {
        BuyEvent -= Sell;
        BuyEvent -= Pay;
    }
}
