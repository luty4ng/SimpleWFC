using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 center;
    public WFCGenerator generator;
    public float rotateSpeed = 50f;
    public float moveSpeed = 10f;
    public float sensitivity = 10f;
    private Camera cam;
    private float horizontal;
    private float vertical;
    private Vector3 movement;
    private static class InitCam
    {
        public static Vector3 position;
        public static Vector3 eularAngle;
        public static float size;
    }

    void Start()
    {
        cam = this.GetComponent<Camera>();
        InitCam.position = cam.transform.position;
        InitCam.eularAngle = cam.transform.rotation.eulerAngles;
        InitCam.size = cam.orthographicSize;
    }



    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        center = generator.mapSize / 2;
        Movement();
        if (Input.GetKey(KeyCode.Q))
            this.transform.RotateAround(center, Vector3.up, rotateSpeed * Time.deltaTime);
        else if (Input.GetKey(KeyCode.E))
            this.transform.RotateAround(center, Vector3.up, -rotateSpeed * Time.deltaTime);
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ResetCam();
        }

        float size = cam.orthographicSize;
        size -= Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        Debug.Log(size);
        size = Mathf.Clamp(size, 5f, 10f);
        cam.orthographicSize = size;

    }

    void Movement()
    {
        movement = new Vector3(horizontal, vertical, 0) * Time.deltaTime * moveSpeed;
        transform.Translate(movement);
    }

    void ResetCam()
    {
        cam.transform.position = InitCam.position;
        cam.transform.eulerAngles = InitCam.eularAngle;
        cam.orthographicSize = InitCam.size;
    }
}
