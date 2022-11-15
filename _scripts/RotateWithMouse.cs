using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RotateWithMouse : MonoBehaviour
{
    public bool isActive = true;
    [SerializeField] GameObject RotateObj;

    [Header("Rotation Settings")]
    [Range(0, 20)]
    [SerializeField] float sensitivity = 1f;
    [SerializeField] Vector2 Radians;
    [Range(60, 88)]
    [SerializeField] float yValueClamp = 0;


    [Header("Border Acceleration")]
    [SerializeField] bool useBorderAcceleration = false;
    [SerializeField] float borderSize = 45f;
    [SerializeField] float borderAcceleration = 1f;
    [SerializeField] Point xBounds = new Point(0, Screen.width);

    Vector2 prevPos;
    private void Start()
    {
        xBounds = new Point((int)borderSize, Screen.width - (int)borderSize);
    }
    // Update is called once per frame
    public void setRotationObj(GameObject obj)
    {
        RotateObj = obj;
    }
    void Update()
    {
        if (isActive)
        {
            Vector3 Eulers = Vector3.zero; 
            Point pos = new Point(Input.mousePosition.x, Input.mousePosition.y);
            bool inAccelerationBounds = false;
            if (useBorderAcceleration &&
                !InBounds(xBounds, pos) &&
                InBounds(new Point(0, Screen.width), pos))
                inAccelerationBounds = true;
            Eulers.x = Radians.x * Input.GetAxis("MouseY");
            Eulers.y = Radians.y * (Input.GetAxis("MouseX") + (inAccelerationBounds ? 1 : 0));

            Eulers = Eulers * Time.deltaTime * sensitivity * (inAccelerationBounds ? borderAcceleration * (pos.x < xBounds.x ? -1 : 1) : 1);
            RotateObj.transform.Rotate(Eulers * Time.deltaTime * sensitivity * (inAccelerationBounds ? borderAcceleration * (pos.x < xBounds.x ? -1 : 1) : 1));
            Mathf.Clamp(RotateObj.transform.rotation.eulerAngles.y, -yValueClamp, yValueClamp);

            prevPos = Input.mousePosition;
        }
    }
    public static bool InBounds(Point A, Point v) 
    {
        return (A.x <= v.x && v.x <= A.y);
    }
}
