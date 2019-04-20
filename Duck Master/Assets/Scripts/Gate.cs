﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : LogicOutput
{
    [SerializeField] 
    private bool active;
    [SerializeField] 
    private Material underTileMat;
    [SerializeField] 
    private GameObject obj;
    [SerializeField] 
    private List<Material> gateMaterial;
    [SerializeField]
    private ParticleSystem[] portalEmissions;
    GameObject tileObj;

    Transform gateTransform;
    MeshRenderer objMeshRenderer;
    Vector3 tilePosition;

    DuckTile.TileType originalType;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        gateTransform = gameObject.transform;
        active = false;
       
        if (obj != null)
        {
            objMeshRenderer = obj.GetComponent<MeshRenderer>();
        }
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    public override void Activate(bool activate)
    {
        active = activate;
        var mat = new Material[2];

        if (active)
        {
            print("Opening gate");
            GameManager.Instance.GetTileMap().getTileFromPosition(tilePosition).mType = originalType;
            if (!portalEmissions[0].isPlaying)
            {
                portalEmissions[0].Play();
                portalEmissions[1].Play();
            }
        }
        else
        {
            //print(tilePosition.ToString());
            GameManager.Instance.GetTileMap().getTileFromPosition(tilePosition).mType = DuckTile.TileType.UnpassableBoth;
            if (portalEmissions[0].isPlaying)
            {
                portalEmissions[0].Stop();
                portalEmissions[1].Stop();
            }
        }
    }

    public bool IsActive()
    {
        return active;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.name == "ground(Clone)")
        {
            other.gameObject.GetComponent<Renderer>().material = underTileMat;
            UIManager.AddUnderGateTile(other.gameObject);
            print("colliding with ground tile");
            tilePosition = other.gameObject.transform.position;
            originalType = GameManager.Instance.GetTileMap().getTileFromPosition(tilePosition).mType;
            Activate(active);
        }
    }
}
