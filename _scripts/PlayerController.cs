using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.Netcode;
using UnityEngine;


public class PlayerController : AnimatableEntity
{
    [SerializeField] protected Cinemachine.AxisState yAxisRotation;
    [SerializeField] protected Cinemachine.AxisState xAxisRotation;

    [Tooltip("Limits vertical camera rotation. Prevents the flipping that happens when rotation goes above 90.")]
    [Range(0f, 90f)][SerializeField] float yRotationLimit = 88f;

    Vector2 rotation = Vector2.zero;


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        yAxisRotation = new AxisState(-180, 180, true, false, 500, 0, 0, "MouseX", false);
        xAxisRotation = new AxisState(-yRotationLimit, yRotationLimit, false, false, 500, 0, 0, "MouseY", true);
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    public override void Update()
    {
        base.Update();
        yAxisRotation.Update(Time.deltaTime);
        xAxisRotation.Update(Time.deltaTime);
        rbody.MoveRotation(Quaternion.Euler(xAxisRotation.Value, yAxisRotation.Value, 0f));
    }
    public override void rotateTowards(Vector3 direction)
    {
    }
    public void SetRotAxis(float x, float y)
    {
        yAxisRotation.Value = y;
        xAxisRotation.Value = x;
    }
    public Vector2 GetRotAxis()
    {
        return new Vector2(xAxisRotation.Value, yAxisRotation.Value);
    }
    public override Vector2 getMoveInput()
    {
        Vector3 move = Input.GetAxis("Vertical") * transform.forward + Input.GetAxis("Horizontal") * transform.right;
        Vector2 convertedMove = new Vector2(move.x, move.z);
        return convertedMove;
    }

    public override Vector2 HandleGroundedInput()
    {
        return getMoveInput();
    }

    public override Vector2 HandleAirInput()
    {
        return getMoveInput();
    }

    public override void OnDie()
    {
        throw new System.NotImplementedException();
    }

    public override void OnGrounded()
    {

    }

    public void SetAxisClamp(int index, float min, float max, bool wrap = false)
    {
        AxisState axis;
        switch(index)
        {
            case 0:
                axis = xAxisRotation;
                break;
            case 1:
                axis = yAxisRotation;
                break;
            default:
                return;
                
        }
        axis.m_MinValue = min;
        axis.m_MaxValue = max;
        axis.m_Wrap = wrap;
    }
}
