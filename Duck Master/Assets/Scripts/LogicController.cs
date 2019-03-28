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

    // Start is called before the first frame update
    void Start()
    {
        inputs = transform.GetComponentsInChildren<LogicInput>();
        outputs = transform.GetComponentsInChildren<LogicOutput>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mode == LogicMode.AND)
        {
            bool activate = true;
            foreach(LogicInput input in inputs)
            {
                if (!input.IsActive())
                    activate = false;
            }
			
			if(activate)
				print("And mode activated");
			
			foreach(LogicOutput output in outputs)
			{
				output.Activate(activate);
			}
        }
		
		if (mode == LogicMode.OR)
		{
			bool activate = false;
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
			
			foreach(LogicOutput output in outputs)
			{
				output.Activate(activate);
			}
		}
		
		if (mode == LogicMode.NOT)
		{
			bool activate = true;
			
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
			
			foreach(LogicOutput output in outputs)
			{
				output.Activate(activate);
			}
		}
    }
}
