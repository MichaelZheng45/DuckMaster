using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface LogicInput
{
    bool IsActive();
}

public interface LogicOutput
{
    void Activate(bool active);
}

public enum LogicMode
{
    AND,
    OR,
    NOT
}

public class LogicController : MonoBehaviour
{
    [SerializeField] LogicMode mode = LogicMode.AND;
    LogicInput[] inputs;
    LogicOutput[] outputs;
    bool activate;
    bool lastChange;

    // Start is called before the first frame update
    void Start()
    {
        inputs = transform.GetComponentsInChildren<LogicInput>();
        outputs = transform.GetComponentsInChildren<LogicOutput>();

        if (inputs == null)
            print("ERROR: input is null");
        if (outputs == null)
            print("ERROR: outputs is null");

    }

    // Update is called once per frame
    void Update()
    {
        if (mode == LogicMode.AND)
        {
            print("In and mode");
            activate = true;
            foreach(LogicInput input in inputs)
                if (!input.IsActive())
                    activate = false;

            if (activate)
                print("And mode activated");
            else
                print("And mode deactivated");
			
        }
		
		if (mode == LogicMode.OR)
		{
			activate = false;
			foreach(LogicInput input in inputs)
			{
				if (input.IsActive())
				{
					activate = true;
					break;
				}
			}

            if (activate)
                print("OR Mode activated");
            else
                print("Or mode deactivated");
			
		}
		
		if (mode == LogicMode.NOT)
		{
            activate = true;
			foreach(LogicInput input in inputs)
			{
				if (input.IsActive())
				{
					activate = false;
					break;
				}
			}

            if (!activate)
                print("NOT mode activated");
            else
                print("Not mode deactivated");
		}

        //Final set
        foreach (LogicOutput output in outputs)
             output.Activate(activate);
         
    }
}
