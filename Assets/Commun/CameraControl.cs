using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float speed;
    
    public KeyCode up;
    public KeyCode down;
    public KeyCode left;
    public KeyCode right;
    public KeyCode front;
    public KeyCode back;

    private Vector2 clampX = new Vector2(-89, 89);
    private Vector2 rotation = new Vector2(0, 0);

    bool IsMouseOverGameWindow { 
        get { return !(0 > Input.mousePosition.x || 0 > Input.mousePosition.y || Screen.width < Input.mousePosition.x || Screen.height < Input.mousePosition.y); } 
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Cursor.visible == true)
        {
            if (IsMouseOverGameWindow && Input.GetMouseButton(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            return;
        }

        Transform camTransform = Camera.main.transform;

        //update rotation
        rotation.y += Input.GetAxis("Mouse X") * speed/3;
        rotation.x += -Input.GetAxis("Mouse Y") * speed/3;
        rotation.x = Mathf.Clamp(rotation.x, clampX.x, clampX.y);
        transform.eulerAngles = rotation * speed;
        camTransform.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, 0);

        //update position
        if (Input.GetKey(front))
        {
            camTransform.position += new Vector3(camTransform.forward.x * Time.deltaTime * speed, camTransform.forward.y * Time.deltaTime * speed, camTransform.forward.z * Time.deltaTime * speed);
        }
        if (Input.GetKey(back))
        {
            camTransform.position -= new Vector3(camTransform.forward.x * Time.deltaTime * speed, camTransform.forward.y * Time.deltaTime * speed, camTransform.forward.z * Time.deltaTime * speed);
        }
        if (Input.GetKey(left))
        {
            camTransform.position += Vector3.Cross(Camera.main.transform.forward, Camera.main.transform.up) * Time.deltaTime * speed;
        }
        if (Input.GetKey(right))
        {
            camTransform.position -= Vector3.Cross(Camera.main.transform.forward, Camera.main.transform.up) * Time.deltaTime * speed;
        }
        if (Input.GetKey(up))
        {
            camTransform.position += new Vector3(camTransform.up.x * Time.deltaTime * speed, camTransform.up.y * Time.deltaTime * speed, camTransform.up.z * Time.deltaTime * speed);
        }
        if (Input.GetKey(down))
        {
            camTransform.position -= new Vector3(camTransform.up.x * Time.deltaTime * speed, camTransform.up.y * Time.deltaTime * speed, camTransform.up.z * Time.deltaTime * speed);
        }
    }
}
