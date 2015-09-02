using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Expression
{
	/* Variables */
	public int type; // 0: number 1: number reference 2: operation
	public int number;
	public Expression leftExp;
	public Expression rightExp;
	public int op; // 0: + 1: - 2: * 3: /
	public string entRef;
	public string numRef;

	/* Functions */
	public Expression(int _type)
	{
		type = _type;
		number = 0;
		leftExp = null;
		rightExp = null;
		op = 0;
		entRef = "";
		numRef = "";
	}
	
	public int evaluate(StoryWorld sw, Verb vc)
	{
		if (type == 2)
		{
			int left = leftExp.evaluate(sw, vc);
			int right = rightExp.evaluate(sw, vc);

			switch(op)
			{
			case 0: return left + right;
			case 1: return left - right;
			case 2: return left * right;
			case 3: 
				{
					if (right == 0)
						return 0; //Treason, I know, but I'm too lazy to think up the proper solution
					else
						return left / right;
				}
			}
		}
		else if (type == 0)
			return number;
		else if (type == 1)
		{
			if (entRef[0] == '?' && vc != null)
				return vc.variables.Find(x => x.name == entRef).values[0].numbers.Find(y => y.name == numRef).value; 
			else
				return sw.entities.Find(x => x.name == entRef).numbers.Find(y => y.name == numRef).value;
		}
		else
			return 0;
		return 0;
	}

	public void replaceWith(string replace, string with)
	{
		if (leftExp != null)
			leftExp.replaceWith(replace, with);
		if (rightExp != null)
			rightExp.replaceWith(replace, with);
		if (replace == entRef)
			entRef = with;
	}

	public void copyTo(Expression e)
	{
		e.type = type;
		e.number = number;

		if (leftExp != null)
		{
			e.leftExp = new Expression(leftExp.type);
			leftExp.copyTo(e.leftExp);
		}
		else
			e.leftExp = null;

		if (rightExp != null)
		{
			e.rightExp = new Expression(rightExp.type);
			rightExp.copyTo(e.rightExp);
		}
		else
			e.rightExp = null;

		e.op = op;
		e.entRef = entRef;
		e.numRef = numRef;
	}
}
