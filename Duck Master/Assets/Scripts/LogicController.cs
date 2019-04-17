using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicInput : MonoBehaviour
{
    public virtual bool IsActive() { return false; }
    public void CallChange()
    {
        foreach(LogicOutput output in outputList)
            output.CheckState();
    }
    [SerializeField] List<LogicOutput> outputList;

    public List<LogicOutput> GetOutputs()
    {
        return outputList;
    }
}

public class LogicOutput : MonoBehaviour
{
    public virtual void Activate(bool active) { }
    [SerializeField] 
    private List<LogicInput> AND_List;
    [SerializeField] 
    private List<LogicInput> OR_List;
    [SerializeField]
    private List<LogicInput> NOT_List;


    //Give all inputs a reference to the output object
    public void Start()
    {
        if (AND_List.Count > 0)
        {
            foreach (LogicInput input in AND_List)
                input.GetOutputs().Add(this);
        }

        if (OR_List.Count > 0)
        {
            foreach (LogicInput input in OR_List)
                input.GetOutputs().Add(this);
        }

        if (NOT_List.Count > 0)
        {
            foreach (LogicInput input in NOT_List)
                input.GetOutputs().Add(this);
        }

    }

    //Something tells me this is insanely over-engineered, and/or unnecessary but oh well.
    public void Update()
    {
        
    }

    public void CheckState()
    {
        //print("Calling Check State Logic");
        bool and = false, or = false, not = false;

        //AND
        if (AND_List.Count > 0)
        {
            and = true;

            foreach (LogicInput input in AND_List)
            if (!input.IsActive())
            {
                and = false;
                break;
            }
        }


        //OR
        if (OR_List.Count > 0)
        {
            or = false;

            foreach (LogicInput input in OR_List)
            {
                if (input.IsActive())
                {
                    or = true;
                }
            }
        }

        //NOT
        if (NOT_List.Count > 0)
        {
            not = true;

            foreach (LogicInput input in NOT_List)
            {
                if (input.IsActive())
                {
                    not = false;
                    break;
                }

            }
        }

        Activate(and || or || not);
    }

}

public enum LogicMode
{
    NONE,
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
            activate = true;
            foreach (LogicInput input in inputs)
                if (!input.IsActive())
                    activate = false;
        }

        if (mode == LogicMode.OR)
        {
            activate = false;
            foreach (LogicInput input in inputs)
            {
                if (input.IsActive())
                {
                    activate = true;
                    break;
                }
            }
        }

        if (mode == LogicMode.NOT)
        {
            activate = true;
            foreach (LogicInput input in inputs)
            {
                if (input.IsActive())
                {
                    activate = false;
                    break;
                }
            }
        }

        //Final set
        foreach (LogicOutput output in outputs)
            output.Activate(activate);

    }
}
