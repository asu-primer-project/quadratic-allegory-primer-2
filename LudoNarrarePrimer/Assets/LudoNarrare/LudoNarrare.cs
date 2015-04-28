using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Xml;

public class LudoNarrare : MonoBehaviour
{
    private StoryWorld sw;
    private Engine engine;
	public TextAsset storyWorldFile;
	private int loadStatus;
	private bool done = false;

	//Access functions
	public StoryWorld getStoryWorld()
	{
		return sw;
	}

	public Engine getEngine()
	{
		return engine;
	}

	public bool getDone()
	{
		return done;
	}

	//On start, initialize
	void Start() 
	{
		StoryWorldLoader swLoader = new StoryWorldLoader();
		loadStatus = swLoader.readStoryWorld(storyWorldFile);
		sw = swLoader.storyWorld;

		if (loadStatus == -1)
		{
			engine = new Engine(sw);
			engine.init();
		}
		else
		{
			engine = new Engine(sw);
			Page p = new Page("StandstillError");
			p.drawList.Add(new DrawInstruction(true, "", "", "Error in LNScript file on line " + loadStatus, 0, 0));
			engine.output.Add(p);
		}
		done = true;
	}
}
