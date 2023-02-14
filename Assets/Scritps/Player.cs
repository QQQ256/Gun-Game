using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
    public float moveSpeed = 5f;

    public CrossHairs crossHairs;

    Camera viewCamera;
    PlayerController controller;
    GunController gunController;

    protected override void Start()
    {
        base.Start();
        gunController = GetComponent<GunController>();
        controller = GetComponent<PlayerController>();
        viewCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // movement input
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")); // GetAxis is smoothed based on the “sensitivity” setting so that value gradually changes from 0 to 1, or 0 to -1. Whereas GetAxisRaw will only ever return 0, -1, or 1 exactly
        Vector3 moveVelocity = moveInput.normalized* moveSpeed;
        controller.Move(moveVelocity);

        // look input
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GetWeaponHeight); // 它的法线方向为Vector3.up，即为y轴正方向，它的原点为Vector3.zero，即为原点。这个Plane对象可以用来检测物体和地面的碰撞或者其他用途。
        float rayDistance;

        /*
        如果射线和平面相交，那么我们可以使用射线的起点和射线的方向来计算出交点的坐标。

        射线的起点表示射线的起始位置，射线的方向表示射线的运动方向。

        在这个例子里，射线的起点是ray.origin，射线的方向是ray.direction。

        当射线和平面相交时，我们可以使用射线起点加上射线方向乘以距离得到交点的坐标。这是因为交点的坐标等于射线起点加上射线方向乘以射线与平面的距离。

        具体来说:

        交点 = 射线起点 + 射线方向 * 距离

        或

        交点 = ray.origin + ray.direction * rayDistance

        这里的rayDistance就是射线与平面的距离，可以使用Plane.Raycast()函数得到。

        在上面的代码中,
        if(groundPlane.Raycast(ray, out rayDistance))
        Vector3 point = ray.GetPoint(rayDistance);

        这句话就是根据射线和平面的距离计算出交点的坐标。
        */
        if(groundPlane.Raycast(ray, out rayDistance)){
            Vector3 point = ray.GetPoint(rayDistance);
            Debug.DrawLine(ray.origin, point, Color.red);
            controller.LookAt(point);

            crossHairs.transform.position = point;
            crossHairs.DetectEnemy(ray);
        }

        // weapon input
        if(Input.GetMouseButton(0)){
            gunController.OnTriggerHold();
        }
        if(Input.GetMouseButtonUp(0)){
            gunController.OnTriggerRelease();
        }
    }
}
