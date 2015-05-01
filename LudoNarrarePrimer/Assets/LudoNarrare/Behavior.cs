using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Behavior
{
    /* Variables */
    public string name;
    public string verb;
    public int chance;
    public List<string> arguments;

    /* Functions */
    public Behavior(string _name, string _verb, int _chance)
    {
        name = _name;
        verb = _verb;
        chance = _chance;
        arguments = new List<string>();
        //arguments.Add("?me");
    }

    public void copyTo(Behavior b)
    {
        if (b != null)
        {
            b.name = name;
            b.verb = verb;
            b.chance = chance;
            b.arguments = new List<string>();
            for (int i = 0; i < arguments.Count; i++)
                b.arguments.Add(arguments[i]);
        }
    }
}
