using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]    
public class ToggleObj : MonoBehaviour
{
    [SerializeField]
    List<GameObject> objects = new List<GameObject>();
    [SerializeField]
    private bool toggle = false;
    private bool keyDown = false;
    [SerializeField]
    string inputName = "";

    void Update()
    {
        if (Input.GetAxis(inputName) > 0 && !keyDown)
        {
            keyDown = true;
            toggle = !toggle;
            foreach (GameObject obj in objects)
                obj.SetActive(toggle);
        }
        else if (Input.GetAxis(inputName) == 0)
            keyDown = false;
    }
    public void setToggle(bool t)
    {
        toggle = t;
    }
}
