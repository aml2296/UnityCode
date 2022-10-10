using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayClick : MonoBehaviour
{
    Camera cam;
    Vector3 pos;
    [SerializeField]
    LayerMask mask;
    [SerializeField]
    [Range(0.1f, 50)]
    float power = 30f;
    RaycastHit hit;
    // Start is called before the first frame update
    GameObject selection;
    [SerializeField]
    private Material outlineMaterial;
    [SerializeField]
    private float outlineSize;
    
    void Start()
    {
        cam = Camera.main;
    }
    void Update()
    {
        pos = Input.mousePosition;
        if (IsInScreen(pos))
        {
            Ray ray = cam.ScreenPointToRay(pos);
            Debug.DrawRay(ray.origin, ray.direction * 30, Color.yellow);
            if (Physics.Raycast(ray, out hit, power, mask))
            {
                TileColumn tC = hit.transform.GetComponentInParent<TileColumn>();
                TileChunk chunk = tC.GetComponentInParent<TileChunk>();
               // Debug.Log(chunk.getPosition.ToString() + " = " + tC.gameObject.name);
                if (selection != hit.transform.gameObject)
                {
                    if (selection)
                        GameObject.Destroy(selection);
                    selection = hit.transform.gameObject;
                    GameObject outline = new GameObject("Outline");
                    MeshRenderer mR = outline.AddComponent<MeshRenderer>();
                    mR.material = outlineMaterial;
                    MeshFilter filter = outline.AddComponent<MeshFilter>();
                    filter.mesh = selection.GetComponent<MeshFilter>().mesh;
                    outline.transform.parent = selection.transform;
                    outline.transform.localScale = Vector3.one * outlineSize;
                    outline.transform.localPosition = Vector3.zero;                    
                    selection = outline;
                }
            }
        }
    }

    public static bool IsInScreen(Vector3 pos)
    {
        return pos.x >= 0 && pos.x <= Screen.width && pos.y >= 0 && pos.y <= Screen.height;
    }
}
