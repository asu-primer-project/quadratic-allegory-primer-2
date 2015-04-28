using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Relationship
{
    /* Variables */
    public string name;
    public string other;

    /* Functions */
    public Relationship(string _name, string _other)
    {
        name = _name;
        other = _other;
    }

    public void copyTo(Relationship r)
    {
        if (r != null)
        {
            r.name = name;
            r.other = other;
        }
    }
}
