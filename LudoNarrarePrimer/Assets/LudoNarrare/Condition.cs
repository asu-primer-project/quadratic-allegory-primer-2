using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Condition
{
    /* Variables */
	public bool notFlag;
	public AtomicCondition atomic;
	public Condition left;
	public Condition right;
	public bool andFlag;
	public bool orFlag;

    /* Functions */
    public Condition()
    {
		notFlag = false;
		atomic = null;
		left = null;
		right = null;
		andFlag = false;
		orFlag = false;
    }

	public bool evaluate(StoryWorld sw, Verb vc)
	{
		if (notFlag && left != null)
		{
			bool t = left.evaluate(sw, vc);

			if (t)
				return false;
			else
				return true;
		}
		else if (atomic != null)
		{
			bool t = atomic.evaluate(sw, vc);

			if (t)
				return true;
			else
				return false;
		}
		else if (left != null && right != null)
		{
			bool l = left.evaluate(sw, vc);
			bool r = right.evaluate(sw, vc);

			if (andFlag)
			{
				if (l && r)
					return true;
				else
					return false;
			}
			else if (orFlag) 
			{
				if (l || r)
					return true;
				else
					return false;
			}
			else
				return false;
		}
		else
			return false;
	}

    public void replaceWith(string replace, string with)
    {
		if (atomic != null)
			atomic.replaceWith(replace, with);
		if (left != null)
			left.replaceWith(replace, with);
		if (right != null)
			right.replaceWith(replace, with);
    }

    public void copyTo(Condition c)
    {
        if (c != null)
        {
			c.notFlag = notFlag;

			if (atomic != null)
			{
				c.atomic = new AtomicCondition();
				atomic.copyTo(c.atomic);
			}
			else
				c.atomic = null;

			if (left != null)
			{
				c.left = new Condition();
				left.copyTo(c.left);
			}
			else
				c.left = null;

			if (right != null)
			{
				c.right = new Condition();
				right.copyTo(c.right);
			}
			else
				c.right = null;

			c.andFlag = andFlag;
			c.orFlag = orFlag;
        }
    }
}
