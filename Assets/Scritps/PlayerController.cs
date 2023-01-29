using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    Vector3 velocity;
    Rigidbody myRigidBody;

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody>();
    }

    // update rigidbody position
    private void FixedUpdate() {
        myRigidBody.MovePosition(myRigidBody.position + velocity * Time.fixedDeltaTime);
    }

    public void Move(Vector3 _velocity){
        velocity = _velocity;
    }

    public void LookAt(Vector3 lookPoint){
        Vector3 rightPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z); // 限定高度，否则会倾斜角度去看lookPoint
        transform.LookAt(rightPoint);
    }
}
