using System;
using System.Collections.Generic;

namespace GirlsDevGames.MassiveAI
{
/// <summary>
/// Runs steps one by one. Each must return true before moving to the next.
/// Completes when all steps succeed.
/// </summary>
class Sequence
{
	private readonly List<Func<bool>> steps;
	private int index;

	public Sequence(params Func<bool>[] steps)
	{
		this.steps = new List<Func<bool>>(steps);
	}

	public bool Tick()
	{
		if (index < steps.Count)
		{
			if (steps[index]())
			{
				index++;      // step completed
			}
			// return false if still running or if just finished this step,
			// because the next step will only start on the next Tick()
			return false;
		}

		// All steps completed
		return true;
	}

	public void Reset() => index = 0;
}

/// <summary>
/// Runs all steps each tick. Completes only when all succeed.
/// </summary>
class Parallel
{
	private readonly List<Func<bool>> steps;
	private readonly HashSet<int> completed = new();

	public Parallel(params Func<bool>[] steps)
	{
		this.steps = new List<Func<bool>>(steps);
	}

	public bool Tick()
	{
		for (int i = 0; i < steps.Count; i++)
		{
			if (completed.Contains(i)) continue;
			if (steps[i]()) completed.Add(i);
		}
		return completed.Count == steps.Count;
	}

	public void Reset() => completed.Clear();
}

/// <summary>
/// Factory sugar for building flows that compose.
/// </summary>
public static class FlowNodes
{
	public static Func<bool> Seq(params Func<bool>[] steps)
	{
		var seq = new Sequence(steps);
		return () => seq.Tick();
	}

	public static Func<bool> Par(params Func<bool>[] steps)
	{
		var par = new Parallel(steps);
		return () => par.Tick();
	}
}}
