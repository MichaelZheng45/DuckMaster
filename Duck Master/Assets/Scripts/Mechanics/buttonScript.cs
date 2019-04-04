using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonScript : LogicInput
{
	bool pressed = false;
	[SerializeField]float waitPeriod;
	float timer;

	[SerializeField] Material pressedMat;
	[SerializeField] Material unPressed;

	MeshRenderer renderer;
    // Start is called before the first frame update
    void Start()
    {
		renderer = gameObject.GetComponent<MeshRenderer>();        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug
        //if (Input.GetKeyDown(KeyCode.Alpha4))
        //{
        //    print("button state changed");
        //    pressed = !pressed;
        //}

        if(pressed)
		{
			timer += Time.deltaTime;
			if(waitPeriod < timer)
			{
				timer = 0;
				pressed = false;
				renderer.material = unPressed;
			}
		}
    }

	private void OnTriggerEnter(Collider other)
	{
		string tag = other.gameObject.tag;
		if (pressed == false &&(tag == "Duck" || tag == "Player"))
		{
			pressed = true;
			renderer.material = pressedMat;
			//GameManager.Instance.buttonActivated();
		}
	}

    public override bool IsActive()
    {
        return pressed;
    }
}
