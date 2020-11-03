using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSens = 0.5f;
    public float zoomSens = 0.5f;
    public float rotateSens = 3;
    public float maxZoom = 10f;
    public float scrollThreshold = 0.01f;   

    Vector3 touchStart;
    private Vector3 oldMousePos;

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0){
            Touch touch = Input.GetTouch(0);
            if (Input.touchCount == 2)
            {
                Touch touch2 = Input.GetTouch(1);

                Vector2 touchPrevPos = touch.position - touch.deltaPosition;
                Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;

                float prevMagnitude = (touchPrevPos - touch2PrevPos).magnitude;
                float currentMagnitude = (touch.position - touch2.position).magnitude;

                float diffence = currentMagnitude - prevMagnitude;
               
                //if scroll is above threshold then scroll else rotate
                if (diffence > scrollThreshold || diffence < -scrollThreshold)
                {
                    float newSize = Mathf.Clamp(Camera.main.orthographicSize - (diffence * zoomSens), 2, maxZoom);
                    Camera.main.orthographicSize = newSize;
                } else {
                    double deltaAngle = Math.Atan2((touch.deltaPosition.y - touch2.deltaPosition.y) , (touch.deltaPosition.x - touch2.deltaPosition.x));

                    transform.Rotate(Vector3.up * -(float)deltaAngle* rotateSens);
                }
            }
            else
            {
                //if(touch.phase == TouchPhase.Began){ oldPos = touch.position; }
                //Vector2 normal = (oldPos - touch.position).normalized;
                //Vector2 move = normal * moveSens * Time.deltaTime * activeCamera.orthographicSize;
                //Vector3 translation = new Vector3(move.x, 0, move.y);
                //transform.Translate(translation);
                //oldPos = touch.position;
                //}
                if (Input.GetMouseButtonDown(0))
                {
                    touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                }
                else if (Input.GetMouseButton(0))
                {
                    Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector3 newPos = transform.position + direction;
                    newPos = new Vector3(Mathf.Clamp(newPos.x, 0, WorldManager.Instance.worldSizeX), Mathf.Clamp(newPos.y, 0, WorldManager.Instance.worldSizeY / 2), Mathf.Clamp(newPos.z, 0, WorldManager.Instance.worldSizeY));
                    transform.position = newPos;

                    //Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    //Vector3 newPos = transform.position + direction;
                    //Vector3 clamped = new Vector3(Mathf.Clamp(newPos.x, 0, WorldManager.Instance.worldSizeX - 1), 0, Mathf.Clamp(newPos.x, 0, WorldManager.Instance.worldSizeY - 1));
                    //transform.position = new Vector3(clamped.x, WorldManager.Instance.GetHeight(new Vector2Int((int)clamped.x, (int)clamped.z)), clamped.z);
                }
            }
        }
        if (Application.isEditor)
        {
            if (Input.GetMouseButtonDown(0))
            {
                touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            else if (Input.GetMouseButton(0))
            {
                Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 newPos = transform.position + direction;
                newPos = new Vector3(Mathf.Clamp(newPos.x, 0, WorldManager.Instance.worldSizeX), Mathf.Clamp(newPos.y, 0, WorldManager.Instance.worldSizeY / 2), Mathf.Clamp(newPos.z, 0, WorldManager.Instance.worldSizeY));
                transform.position = newPos;
                //Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                //Vector3 newPos = transform.position + direction;
                //Vector3 clamped = new Vector3(Mathf.Clamp(newPos.x, 0, WorldManager.Instance.worldSizeX - 1), 0, Mathf.Clamp(newPos.x, 0, WorldManager.Instance.worldSizeY - 1));
                //transform.position = new Vector3(clamped.x, WorldManager.Instance.GetHeight(new Vector2Int((int)clamped.x, (int)clamped.z)), clamped.z);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                float newSize = Mathf.Clamp(Camera.main.orthographicSize - (Input.GetAxis("Mouse ScrollWheel") * zoomSens * 100), 2, maxZoom);
                Camera.main.orthographicSize = newSize;
            }
            else if (Input.GetMouseButtonDown(2))
            {
                oldMousePos = Input.mousePosition;
            }
            else if (Input.GetMouseButton(2))
            {
                Vector3 mousePos = Input.mousePosition;
                float rotationDirection = mousePos.x - oldMousePos.x;
                transform.Rotate(Vector3.up *rotationDirection * rotateSens);

                oldMousePos = mousePos;
            }
        }
    }
}