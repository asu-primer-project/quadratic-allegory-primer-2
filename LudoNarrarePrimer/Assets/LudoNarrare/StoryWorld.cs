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
	public Page input;
    public List<Verb> verbs;
	public List<Ending> endings;

    /* Functions */
    public StoryWorld(string _name, string _userEntity)
    {
        name = _name;
        userEntity = _userEntity;
		beginning = new List<Page>();
        entities = new List<Entity>();
		input = null;
        verbs = new List<Verb>();

        Verb vTemp = new Verb("Wait");
		Case cTemp = new Case("Wait");
		cTemp.page = new Page("Wait");
		vTemp.cases.Add(cTemp);
		Discriminator dTemp = new Discriminator("Never show wait");
		dTemp.neverShow = true;
		vTemp.discriminators.Add(dTemp);
		vTemp.icon = "waitIcon";
        verbs.Add(vTemp);

		endings = new List<Ending>();
    }
}
