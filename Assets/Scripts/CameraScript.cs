using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public IslandScript currentIsland;
    private Vector3 dragOrigin;
    private bool isDragging = false;
    [SerializeField] private Camera myCamera;
    private float FOV = 5;
    private bool buildingMode;
    [SerializeField] private GameObject buildGrid;

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


      
        // building mode
        if (Input.GetKeyDown("b"))
        {
            buildingMode = !buildingMode;
            buildGrid.SetActive(buildingMode);
        }

        if (buildingMode)
        {
            var newPos = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2, Screen.height / 2));
            newPos.z = -0.02f;
            newPos.x = newPos.x - ((newPos.x + 0.64f) % 1.28f) + 0.64f;
            newPos.y = newPos.y - ((newPos.y + 0.64f) % 1.28f) + 0.64f;
            buildGrid.transform.localPosition = newPos;


            if (currentIsland.isTile(newPos.x / 1.28f, newPos.y / 1.28f) && buildGrid.activeSelf)
            {
                buildGrid.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.95f);
            } else
            {
                buildGrid.GetComponent<SpriteRenderer>().color = new Color(0.95f, 0.1f, 0.1f);
            }
        }
    }
}
