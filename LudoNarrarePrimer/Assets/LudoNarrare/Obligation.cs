using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Obligation
{
    /* Variables */
    public string name;
    public string verb;
    public List<string> arguments;

    /* Functions */
    public Obligation(string _name, string _verb)
    {
        name = _name;
        verb = _verb;
        arguments = new List<string>();
        arguments.Add("?me");
    }

    public void copyTo(Obligation ob)
    {
        if (ob != null)
        {
            ob.name = name;
            ob.verb = verb;
            ob.arguments = new List<string>();
            for (int i = 0; i < arguments.Count; i++)
                ob.arguments.Add(arguments[i]);
        }
    }
}
