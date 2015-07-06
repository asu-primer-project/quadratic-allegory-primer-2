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

	/*
	//Should be written for the engine, since it needs full story world access for referencing. Implement via recursion
	public int evaluate()
	{

	}
	*/

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

		if (rightExp != null)
		{
			e.rightExp = new Expression(rightExp.type);
			rightExp.copyTo(e.rightExp);
		}

		e.op = op;
		e.entRef = entRef;
		e.numRef = numRef;
	}
}
