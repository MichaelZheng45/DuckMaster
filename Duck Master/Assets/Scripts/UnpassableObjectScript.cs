using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnpassableObjectScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameManager.Instance.GetTileMap().getTileFromPosition(collision.gameObject.transform.position).mType = DuckTile.TileType.UnpassableBoth;
        GameManager.Instance.GetTileMap().CreateConnections();
    }
}
