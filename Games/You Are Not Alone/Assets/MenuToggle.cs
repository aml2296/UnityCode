using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuToggle : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> menus = new List<GameObject>();
    private Dictionary<string,GameObject> toggleList = new Dictionary<string, GameObject>();

    string currentToggleOn = "Off";
    string prevToggle = "";

    private void Start()
    {
        foreach(GameObject m in menus)
            toggleList.Add(m.name, m);

        if (menus.Count > 0)
            prevToggle = currentToggleOn = menus[0].name;
        else
            prevToggle = currentToggleOn;
    }
    // Update is called once per frame
    void Update()
    {
        if(prevToggle.CompareTo(currentToggleOn) != 0)
        {
            prevToggle = currentToggleOn;
            updateToggle(currentToggleOn);
        }
    }
    public void setFalse(string name)
    {
        if (toggleList.ContainsKey(name))
            toggleList[name].SetActive(false);
    }
    public void setToggle(string name)
    {
        currentToggleOn = name;
    }
    private void updateToggle(string name)
    {
        foreach (GameObject g in toggleList.Values)
            g.SetActive(false);
        if (toggleList.ContainsKey(name))
            toggleList[name].SetActive(true);
    }
}
