using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Relationship
{
    /* Variables */
    public string name;
    public string other;
    public bool exclusive;

    /* Functions */
    public Relationship(string _name, string _other, bool _exclusive)
    {
        name = _name;
        other = _other;
        exclusive = _exclusive;
    }

    public void copyTo(Relationship r)
    {
        if (r != null)
        {
            r.name = name;
            r.other = other;
            r.exclusive = exclusive;
        }
    }
}
