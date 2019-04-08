using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicInput : MonoBehaviour
{
    public virtual bool IsActive() { return false; }
    //[SerializeField] List<LogicOutput> outputlist;
}

public class LogicOutput : MonoBehaviour
{
    public virtual void Activate(bool active) { }
    [SerializeField] 
    private List<LogicInput> AND_List;
    [SerializeField] 
    private List<LogicInput> OR_List;
    [SerializeField]
    private List<LogicInput> NOT_list;

    //Something tells me this is insanely over-engineered, and/or unnecessary but oh well.
    public void Update()
    {
        //print("Logic output update called");

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

            //if (and)
            //    print("AND firing");
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

            //if (or)
            //    print("OR firing");

        }

        //NOT
        if (NOT_list.Count > 0)
        {
            not = true;

            foreach (LogicInput input in NOT_list)
            {
                if (input.IsActive())
                {
                    not = false;
                    break;
                }

            }
            //if (not)
            //    print("NOT firing");
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
            print("In and mode");
            activate = true;
            foreach (LogicInput input in inputs)
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
            foreach (LogicInput input in inputs)
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
            foreach (LogicInput input in inputs)
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
