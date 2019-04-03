using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DuckRotationState
{
	TOP,
	RIGHT,
	DOWN,
	LEFT
};

public class DuckRotation : MonoBehaviour
{
	public DuckRotationState currentRotation { get; set; }

	[Tooltip("A number to fudge the rotation to the base rotation (top)")]
	[SerializeField] int rotationFactor;
  
    void Start()
    {
		//set new rotation
		updateDuckRotation();
    }

	public void rotateDuckToDirection(DuckRotationState direction)
	{
		currentRotation = direction;
		updateDuckRotation();
	}

	void updateDuckRotation()
	{
		switch (currentRotation)
		{
			case DuckRotationState.TOP:
				gameObject.transform.rotation = Quaternion.Euler(new Vector3(0,90 + rotationFactor,0));
				break;
			case DuckRotationState.RIGHT:
				gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0 + rotationFactor, 0));
				break;
			case DuckRotationState.DOWN:
				gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 270 + rotationFactor, 0));
				break;
			case DuckRotationState.LEFT:
				gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 180 + rotationFactor, 0));
				break;
			default:
				break;
		}
	}
}
