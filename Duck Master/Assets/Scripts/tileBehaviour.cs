using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tileBehaviour : MonoBehaviour
{
	[SerializeField] bool buttonActive;
	[SerializeField] tileType tType;
	[SerializeField] GameObject obj;

	[SerializeField] List<Material> gateMaterial;
	Transform objTransform;
	MeshRenderer objMeshRenderer;
    // Start is called before the first frame update
    void Start()
    {
		if(obj != null)
		{
			objTransform = obj.transform;
			objMeshRenderer = obj.GetComponent<MeshRenderer>();
		}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void changedByButton(tileType newType)
	{
		tType = newType;
		if(buttonActive)
		{
			buttonActive = false;
			if(tType == tileType.GateDown)
			{
				var mat = new Material[2];
				mat[0] =gateMaterial[2];
				mat[1] =gateMaterial[0];
				objMeshRenderer.materials = mat;
			}
		}
		else
		{
			buttonActive = true;
			if (tType == tileType.GateUp)
			{
				var mat = new Material[2];
				mat[0] = gateMaterial[2];
				mat[1] = gateMaterial[1];
				objMeshRenderer.materials = mat;
			}
		}
	}
}
