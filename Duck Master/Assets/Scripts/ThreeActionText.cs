using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThreeActionText : MonoBehaviour {

    int pickIndex = 1;
    int throwIndex = 2;
    int whistleIndex = 3;
    public int counter = 1;
    public GameObject pickUpText;
    public GameObject throwText;
    public GameObject whistleText;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TextSwitch()
    {
        counter++;

        if (counter == pickIndex)
        {
            counter++;
            pickUpText.SetActive(true);
            throwText.SetActive(false);
            whistleText.SetActive(false);
        }

        if(counter == throwIndex)
        {
            //counter++;
            pickUpText.SetActive(false);
            throwText.SetActive(true);
            whistleText.SetActive(false);
        }

        if(counter == whistleIndex)
        {
            Invoke("CounterReset", 2f);
            pickUpText.SetActive(false);
            throwText.SetActive(false);
            whistleText.SetActive(true);

        }
    
    }

    void CounterReset()
    {
        counter = 0;
        counter++;
        pickUpText.SetActive(true);
        whistleText.SetActive(false);
    }
}
