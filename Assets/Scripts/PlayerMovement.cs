using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;  

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 _mousePos;
    private Camera _camera;
    private Collider _groundCollider;
    private BoxCollider _puckCollider;

    void Start()
    {
        _camera = Camera.main;
        _groundCollider = GameObject.FindWithTag("Base").GetComponent<BoxCollider>();
        _puckCollider = GameObject.Find("Puck").GetComponentInChildren<BoxCollider>();

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0))
            Movement();
    }

    private void Movement() 
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hitInfo)) return;
        if (hitInfo.collider == (_groundCollider || _puckCollider)) 
        {   
            _mousePos = new Vector3(hitInfo.point.x, 0.15f, hitInfo.point.z);
            transform.position = Vector3.MoveTowards(transform.position, _mousePos, Time.deltaTime * 10);
        }
    }
}