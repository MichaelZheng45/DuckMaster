using System.Collections;
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

        UpdateParticleColor();

    }

    void UpdateParticleColor()
    {
        RenderTexture rs = new RenderTexture(Camera.main.pixelWidth, Camera.main.pixelHeight, 24);
        Camera.main.targetTexture = rs;
        Camera.main.Render();
        RenderTexture.active = rs;

        Texture2D tex = new Texture2D(Camera.main.pixelWidth, Camera.main.pixelHeight, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, Camera.main.pixelWidth, Camera.main.pixelHeight), 0, 0);
        tex.Apply();

        foreach (ParticleSystem ps in portalEmissions)
        {
            RaycastHit rhc;
            Physics.Raycast(new Ray(ps.transform.position + new Vector3(0, .1f, 0), -ps.transform.up), out rhc);
            Debug.DrawLine(ps.transform.position + new Vector3(0, 0.1f, 0), rhc.point, Color.red, 1000);
            Debug.Log(Camera.main.WorldToScreenPoint(rhc.point));
            Vector3 VarTemp = Camera.main.WorldToScreenPoint(rhc.point);
            var p = ps.main;
            p.startColor = Color.Lerp(tex.GetPixel((int)VarTemp.x, (int)VarTemp.y), new Color(1, 1, 1, .25f), .25f);
        }

        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate(rs);
        DestroyImmediate(tex);
    }


    // Update is called once per frame
    new void Update()
    {
    }

    public override void Activate(bool activate)
    {
        active = activate;
        var mat = new Material[2];
        GetComponent<Animator>().SetBool("Open", active);
        if (active)
        {
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
            tilePosition = other.gameObject.transform.position;
            originalType = GameManager.Instance.GetTileMap().getTileFromPosition(tilePosition).mType;
            Activate(active);
        }
    }
}
