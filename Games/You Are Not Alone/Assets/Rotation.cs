using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public Vector3 FullRotationPerMinute;
    public bool toggle;
    public bool toggleChange;
    // Start is called before the first frame update

    public void Start()
    {
        toggleChange = !toggleChange;
    }
    private void Update()
    {
        if(toggleChange != toggle)
        {
            toggleChange = toggle;
            if (toggle)
                BeginRotate();
        }
    }
    public void BeginRotate()
    {
        toggle = true;
        if (gameObject.activeSelf)
            StartCoroutine(Rotate());
    }

    IEnumerator Rotate()
    {
        Vector3 rotationSpeed = FullRotationPerMinute / (0.01f*60f);
        while (toggle)
        {
            gameObject.transform.Rotate(rotationSpeed*Time.deltaTime);
            yield return new WaitForSeconds(0.01f);
        }
    }
}
