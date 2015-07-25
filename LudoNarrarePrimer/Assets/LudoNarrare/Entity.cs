using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Entity
{
    /* Variables */
    public string name;
	public string agent;
	public IconText it;
    public List<Tag> tags;
    public List<Relationship> relationships;
	public List<Number> numbers;
	public List<LNString> strings;
	public List<ImageDef> images;

    /* Functions */
    public Entity(string _name)
    {
        name = _name;
		agent = "None";
		it = null;
        tags = new List<Tag>();
        relationships = new List<Relationship>();
		numbers = new List<Number>();
		strings = new List<LNString>();
		images = new List<ImageDef>();
	}
}