using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Entity
{
    /* Variables */
    public string name;
    public List<Tag> tags;
    public List<Relationship> relationships;
	public List<Number> numbers;
	public List<LNString> strings;
    public List<Obligation> obligations;
    public List<Goal> goals;
    public List<Behavior> behaviors;
	public List<ImageDef> images;
	public string icon;
	public IconText it;
	public Page page;

    /* Functions */
    public Entity(string _name)
    {
        name = _name;
        tags = new List<Tag>();
        relationships = new List<Relationship>();
		numbers = new List<Number>();
		strings = new List<LNString>();
        obligations = new List<Obligation>();
        goals = new List<Goal>();
        behaviors = new List<Behavior>();
		images = new List<ImageDef>();
		icon = "";
		page = new Page("entity");
		it = null;
	}
}