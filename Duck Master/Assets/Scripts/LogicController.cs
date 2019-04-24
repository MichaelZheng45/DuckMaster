using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicInput : MonoBehaviour
{
    //public virtual bool IsActive() { return active; }
    public bool IsActive() { return active; }
    public void SetActive(bool act) { active = act; }
    public void SetAdded(bool add) { IsAdded = add; }
    protected bool active;
    bool IsOrdered;
    bool IsAdded = false;
    [SerializeField] List<LogicOutput> outputList;
    //input used for order
    public virtual void CallChange()
    {
        foreach (LogicOutput output in outputList)
        {
            if (IsOrdered)
            {
                if (active && !IsAdded)
                {
                    //output.AddOrderedInput(input);
                    output.AddOrderedInput(this);
                    IsAdded = true;
                }

                //If added to the list and set to false, cancel the operation
                if (!active && IsAdded)
                {
                    print("Sequential Input turned off, clearing and setting adding false");
                    output.ClearReceive();
                    IsAdded = false;
                }
            }

            output.CheckState();
        }
    }
   

    public List<LogicOutput> GetOutputs()
    {
        return outputList;
    }

    public void SetOrder(bool order)
    {
        IsOrdered = order;
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
    [SerializeField]
    private List<LogicInput> Ordered_List;
    List<LogicInput> Received_Inputs; //For order checking

    public void ClearReceive()
    {
        Received_Inputs.Clear();
    }

    public void AddOrderedInput(LogicInput input)
    {
        Received_Inputs.Add(input);
    }

    public void RemoveOrderedInput(LogicInput input)
    {
        Received_Inputs.Remove(input);
    }

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

        if (Ordered_List.Count > 0)
        {
            Received_Inputs = new List<LogicInput>();
            foreach (LogicInput input in Ordered_List)
            { 
                input.GetOutputs().Add(this);
                input.SetOrder(true);
            }
        }

    }

    //Something tells me this is insanely over-engineered, and/or unnecessary but oh well.
    public void Update()
    {
        
    }

    public void CheckState()
    {
        //print("Calling Check State Logic");
        bool and = false, or = false, not = false, order = false;

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

        //Ordered
        if (Ordered_List.Count > 0)
        {
            order = true;

            //Make sure it doesn't fire but don't remove until complete pass
            if (Received_Inputs.Count != Ordered_List.Count) 
                order = false;
            
            else
            {
                bool orderFail = false;

                for (int i = 0; i < Ordered_List.Count; i++)
                {
                    
                    if (Received_Inputs[i] != Ordered_List[i])
                    {
                        print("Out of order inputs!!");
                        order = false;
                        Received_Inputs.Clear();
                        orderFail = true;
                        break;
                    }

                    if (!Ordered_List[i].IsActive())
                    {
                        order = false;
                        Received_Inputs.Clear();
                        orderFail = true;
                        break;
                    }
                }

                if (orderFail)
                {
                    foreach(LogicInput input in Ordered_List)
                    {
                        input.SetActive(false);
                        input.SetAdded(false);
                    }
                }
            }
        }

        Activate(and || or || not || order);
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
