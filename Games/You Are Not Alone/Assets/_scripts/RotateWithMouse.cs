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
            Eulers.z = Radians.x * Input.GetAxis("MouseY");
            Eulers.y = Radians.y * (Input.GetAxis("MouseX") + (inAccelerationBounds ? 1 : 0));

            RotateObj.transform.Rotate(Eulers * Time.deltaTime * sensitivity * (inAccelerationBounds ? borderAcceleration * (pos.x < xBounds.x ? -1 : 1) : 1));

            prevPos = Input.mousePosition;
        }
    }
    public static bool InBounds(Point A, Point v) 
    {
        return (A.x <= v.x && v.x <= A.y);
    }
}
