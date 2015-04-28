using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Tag
{
    /* Variables */
    public string name;

    /* Functions */
    public Tag(string _name)
    {
        name = _name;
    }

    public void copyTo(Tag t)
    {
        if (t != null)
            t.name = name;
    }
}
