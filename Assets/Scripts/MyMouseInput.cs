using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class MyMouseInput : MonoBehaviour
{
    private Camera _camera;
    private Button _capButton;
    private Button _sphereButton;
    private Button _cubeButton;
    private Button _stoneButton;
    private Button _stoneBrickButton;
    private Button _metalButton;
    public Material DefaultMat;
    public Material StoneBricks;
    public Material StoneBlocks;
    public Material MetalBlocks;
    public Material TransparentYellow;
    public Material TransparentGreen;
    private GameObject _playerSelected;

    private bool _isShapeInHand;

    private enum Shape
    {
        None,
        Sphere,
        Cube,
        Capsule
    }

    private Shape _selectedShape = Shape.None;

    private enum Texture
    {
        TransparentYellow,
        TransparentGreen,
        StoneBrick,
        StoneBlock,
        Metal
    }

    private Texture _selectedMaterial = Texture.TransparentYellow;

    private Dictionary<Shape, PrimitiveType> shapeMap = new Dictionary<Shape, PrimitiveType>()
    {
        {Shape.Capsule, PrimitiveType.Capsule},
        {Shape.Cube, PrimitiveType.Cube},
        {Shape.Sphere, PrimitiveType.Sphere},
    };

    private Dictionary<Texture, Material> _materialMap;


    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
        _playerSelected = new GameObject();
        _capButton = GameObject.Find("CapsuleButton").GetComponent<Button>();
        _cubeButton = GameObject.Find("CubeButton").GetComponent<Button>();
        _sphereButton = GameObject.Find("SphereButton").GetComponent<Button>();
        _stoneButton = GameObject.Find("StoneButton").GetComponent<Button>();
        _stoneBrickButton = GameObject.Find("StoneBrick").GetComponent<Button>();
        _metalButton = GameObject.Find("MetalButton").GetComponent<Button>();
        ActivateUI();

        _materialMap = new Dictionary<Texture, Material>()
        {
            {Texture.StoneBlock, StoneBlocks},
            {Texture.TransparentYellow, TransparentYellow},
            {Texture.TransparentGreen, TransparentGreen},
            {Texture.Metal, MetalBlocks},
            {Texture.StoneBrick, StoneBricks},
        };
    }

    void ObjectMovement()
    {
        if (_isShapeInHand)
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                if (hitInfo.transform.tag.Equals("Base"))
                {
                    _playerSelected.transform.position =
                        new Vector3(hitInfo.point.x, hitInfo.point.y + (.5f), hitInfo.point.z);
                    _playerSelected.GetComponent<Renderer>().material = TransparentYellow;
                }
                else if (hitInfo.transform.name == "Cube")
                {
                    _playerSelected.GetComponent<Renderer>().material = TransparentGreen;
                    if (hitInfo.normal == new Vector3(0, 0, 1)) // z+
                    {
                        var position = hitInfo.transform.position;
                        _playerSelected.transform.position =
                            new Vector3(position.x, position.y, hitInfo.point.z + (0.5f));
                    }

                    if (hitInfo.normal == new Vector3(1, 0, 0)) // x+
                    {
                        var position = hitInfo.transform.position;
                        _playerSelected.transform.position =
                            new Vector3(hitInfo.point.x + (0.5f), position.y, position.z);
                    }

                    if (hitInfo.normal == new Vector3(0, 1, 0)) // y+
                    {
                        var position = hitInfo.transform.position;
                        _playerSelected.transform.position =
                            new Vector3(position.x, hitInfo.point.y + (0.5f), position.z);
                    }

                    if (hitInfo.normal == new Vector3(0, 0, -1)) // z-
                    {
                        var position = hitInfo.transform.position;
                        _playerSelected.transform.position =
                            new Vector3(position.x, position.y, hitInfo.point.z - (0.5f));
                    }

                    if (hitInfo.normal == new Vector3(-1, 0, 0)) // x-
                    {
                        var position = hitInfo.transform.position;
                        _playerSelected.transform.position =
                            new Vector3(hitInfo.point.x - (0.5f), position.y, position.z);
                    }
                    if (hitInfo.normal == new Vector3(0, -1, 0)) // y-
                    {
                        var position = hitInfo.transform.position;
                        _playerSelected.transform.position =
                            new Vector3(position.x, hitInfo.point.y - (0.5f), position.z);
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        ObjectMovement();
        if (Input.GetMouseButtonUp(0)) // check if left button is pressed
        {
            PlaceObject();
        }

        if (Input.GetMouseButtonUp(1)) // Function for destroying objects when right-clicked
        {
            RaycastHit hitInfo;
            bool hit = Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out hitInfo);
            Debug.Log(hitInfo.transform.name);
            if (hit && hitInfo.transform.gameObject.GetComponent<TriangleExplosion>())
            {
                StartCoroutine(DestroyObject(hitInfo.transform.gameObject));
            }
        }
    }

    void PlaceObject()
    {
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out hitInfo);
        if (hit && _isShapeInHand)
        {
            _playerSelected.GetComponent<Collider>().enabled = true;
            _isShapeInHand = false;
            if (_selectedMaterial != Texture.TransparentGreen && _selectedMaterial != Texture.TransparentYellow)
            {
                _playerSelected.GetComponent<Renderer>().material = _materialMap[_selectedMaterial];
            }
            else
            {
                _playerSelected.GetComponent<Renderer>().material = DefaultMat;
            }
            //Debug.DrawRay(hitInfo.point, hitInfo.normal, Color.red, 2, false);
            //Debug.Log(hitInfo.normal);
        }
        else
        {
            Debug.Log("No hit");
        }
    }

    public IEnumerator DestroyObject(GameObject obj)
    {
        TriangleExplosion triangleExplosion = obj.GetComponent<TriangleExplosion>();
        StartCoroutine(triangleExplosion.SplitMesh(true));
        yield return new WaitForSeconds(.1f);
    }

    void ActivateUI()
    {
        _capButton.onClick.AddListener(SelectCapsule);
        _sphereButton.onClick.AddListener(SelectSphere);
        _cubeButton.onClick.AddListener(SelectCube);
        _stoneButton.onClick.AddListener(SelectStoneBlock);
        _stoneBrickButton.onClick.AddListener(SelectStoneBrick);
        _metalButton.onClick.AddListener(SelectMetal);
    }

    void SelectStoneBrick()
    {
        _selectedMaterial = Texture.StoneBrick;
    }

    void SelectStoneBlock()
    {
        _selectedMaterial = Texture.StoneBlock;
    }

    void SelectMetal()
    {
        _selectedMaterial = Texture.Metal;
    }

    void CreateObject()
    {
        // Destroy(_playerSelected.GetComponent<Collider>());
        if (_isShapeInHand)
        {
            Destroy(_playerSelected);
        }
        
        var obj = GameObject.CreatePrimitive(shapeMap[_selectedShape]);
        obj.transform.position = new Vector3(100f, 100f, 100f);
        obj.GetComponent<Collider>().enabled = false;
        DefaultMat = obj.GetComponent<Renderer>().material;
        obj.GetComponent<Renderer>().material = TransparentYellow;
        obj.AddComponent<TriangleExplosion>();
        _playerSelected = obj;
    }

    void SelectCube()
    {
        _selectedShape = Shape.Cube;
        CreateObject();
        _isShapeInHand = true;
    }

    void SelectSphere()
    {
        _selectedShape = Shape.Sphere;
        CreateObject();
        _isShapeInHand = true;
    }

    void SelectCapsule()
    {
        _selectedShape = Shape.Capsule;
        CreateObject();
        _isShapeInHand = true;
    }
}