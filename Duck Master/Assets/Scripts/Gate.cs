using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : LogicOutput
{
    [SerializeField] bool active;
    [SerializeField] Material underTileMat;
    [SerializeField] GameObject obj;
    [SerializeField] List<Material> gateMaterial;
    GameObject tileObj;
    Transform objTransform;
    MeshRenderer objMeshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        active = false;
        if (obj != null)
        {
            objTransform = obj.transform;
            objMeshRenderer = obj.GetComponent<MeshRenderer>();
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public override void Activate(bool activate)
    {
        active = activate;
		var mat = new Material[2];
		if (active)
        {

            
            print("Opening gate");
			GameManager.Instance.tileMap.mHeightMap.GetTile(Mathf.FloorToInt(objTransform.position.x), Mathf.FloorToInt(objTransform.position.z)).mType = DuckTile.TileType.PassableBoth; 
            

            mat[0] = gateMaterial[2];
            mat[1] = gateMaterial[0];
            objMeshRenderer.materials = mat;
        }
        else
        {
            print("Closing Gate");
			GameManager.Instance.tileMap.mHeightMap.GetTile(Mathf.FloorToInt(objTransform.position.x), Mathf.FloorToInt(objTransform.position.z)).mType = DuckTile.TileType.UnpassableBoth;
            mat[0] = gateMaterial[2];
            mat[1] = gateMaterial[1];
            objMeshRenderer.materials = mat;
        }
    }

    public bool IsActive()
    {
        return active;
    }

    private void OnTriggerEnter(Collider other)
    {
		/*
        if (other.gameObject.name == "ground(Clone)")
        {
            other.gameObject.GetComponent<Renderer>().material = underTileMat; 
            print("colliding with ground tile");
            tileObj = other.gameObject;
            theTile = GameManager.Instance.GetTilingSystem().getToTileByPosition(other.gameObject.transform.position);
            theTile.tType = tileType.GateUp;
        } */
    }
}
