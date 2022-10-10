using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Item : MonoBehaviour
{
    public event Action PickUp;

    [SerializeField]
    protected int itemId = -1;
    [SerializeField]
    protected int value = 1;
    [SerializeField]
    protected AudioSource source;
    [SerializeField]
    protected MeshRenderer meshRenderer;
    [SerializeField]
    protected Collider[] colliders;

    public void Start()
    {
        try
        {
            if (!meshRenderer)
                meshRenderer = GetComponent<MeshRenderer>();
            if (!meshRenderer)
                meshRenderer = GetComponentInChildren<MeshRenderer>();
            if (!meshRenderer)
                throw new Exception("No Mesh to hide!!!");

                colliders = GetComponents<Collider>();
            if (colliders == null)
                throw new Exception("No Colliders to hide!!!");
            if (!source)
                source = GetComponent<AudioSource>();
        }
        catch(Exception e)
        {
            Debug.LogException(e);
        }
        PickUp += HideDestroy;
        PickUp += PlaySound;
    }
    public void OnDestroy()
    {
        PickUp -= PlaySound;
        PickUp -= HideDestroy;
    }
    public void HideDestroy()
    {
        meshRenderer.enabled = false;
        foreach(Collider c in colliders)
        {
            c.enabled = false;
        }
        Destroy(gameObject, 2);
    }
    public void InvokePickUp()
    {
        PickUp?.Invoke();
    }
    public void PlaySound()
    {
        if (!source)
            return;

        source.Play();
    }
    public int getID()
    {
        return itemId;
    }
    public int getValue()
    {
        return value;
    }
}
