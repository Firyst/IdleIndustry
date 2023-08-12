using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private Vector3 dragOrigin;
    private bool isDragging = false;
    [SerializeField] private Camera myCamera;
    private float FOV = 5;

    void Update()
    {
        myCamera.orthographicSize = FOV;


        // camera drag
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dragOrigin.z = 0;
            isDragging = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentPos.z = 0;
            Vector3 move = dragOrigin - currentPos;
            transform.position += move;
        }

        // camera zoom
        float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
        FOV = Mathf.Clamp(zoomDelta * 4 + FOV, 0.25f, 128);
    }
}
