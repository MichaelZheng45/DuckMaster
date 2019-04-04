using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraControls : MonoBehaviour
{
	public float camSpeed;
	// Start is called before the first frame update

	bool mouseLeftClick;
	bool mouseRightClick;
    bool holdinDuck;
    int mask = 1 << 9; //9th layer mask
    int duckMask = 1 << 10; // 10th layer mask
    void Start()
    {
        
    }
	
	// Update is called once per frame
	//right click to move around
	void Update()
	{
		/*
        holdinDuck = GameManager.Instance.checkIsHoldingDuck();
		if(Input.GetMouseButtonDown(0))
		{
		    mouseLeftClick = true;
            if (!holdinDuck)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, duckMask))
                {
                    //grab the duck 
					GameManager.Instance.clickOnDuck();
                }
            }
            else
            {

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, mask))
                {
                    //check tile if the duck can be thrown on it and throw duck
					
                    GameManager.Instance.throwDuck(hit);
                }
            }
		}
		else
		{
			mouseLeftClick = false;
		}

		if (Input.GetMouseButtonDown(1))
		{
			mouseRightClick = true;
		}
		else
		{
			mouseRightClick = false;
		}
		mouseHover();
		*/
	}

	private void FixedUpdate()
	{
		cameraTransform();
	}

    //for hovering and left click throwing
	void mouseHover()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 100, mask)) 
		{
            //**TO DO***//
            //highlight area
            //if there is a click  right tell game manager and bool that is hovering over tiles.
            //if the player is holding
            GameManager.Instance.movePlayerTo(hit, mouseRightClick);
		}	
	}

	void cameraTransform()
	{
		float rotation = gameObject.transform.rotation.eulerAngles.y;
		Vector3 forwardDirection = new Vector3(Mathf.Cos((rotation - 90) * Mathf.Deg2Rad), 0f, -Mathf.Sin((rotation - 90) * Mathf.Deg2Rad));
		Vector3 sideDirection = new Vector3(Mathf.Cos(rotation * Mathf.Deg2Rad), 0f, -Mathf.Sin(rotation * Mathf.Deg2Rad));

		Vector3 direction = new Vector3(0, 0, 0);
		if (Input.mousePosition.y >= Screen.height * 0.95) //up
		{
			direction += forwardDirection;
		}
		else if (Input.mousePosition.y <= Screen.height * .05)  //down
		{
			direction -= forwardDirection;
		}

		if (Input.mousePosition.x >= Screen.width * 0.95) //left
		{
			direction += sideDirection;
		}
		else if (Input.mousePosition.x <= Screen.width * .05) //right
		{
			direction -= sideDirection;
		}

		direction.Normalize();

		transform.position += direction * camSpeed;
	}
}
