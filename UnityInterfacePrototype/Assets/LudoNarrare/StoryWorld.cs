using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class StoryWorld
{
    /* Variables */
    public string name;
    public string userEntity;
	public List<Page> beginning;
    public List<Entity> entities;
    public List<Verb> verbs;
	public List<Ending> endings;

    /* Functions */
    public StoryWorld(string _name, string _userEntity)
    {
        name = _name;
        userEntity = _userEntity;
		beginning = new List<Page>();
        entities = new List<Entity>();
        verbs = new List<Verb>();

        Verb vTemp = new Verb("!Wait");
        verbs.Add(vTemp);

		endings = new List<Ending>();
    }
}
