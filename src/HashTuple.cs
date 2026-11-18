using System;

namespace GameLauncher;

private class HashTuple
{
	public string Old;

	public string New;

	public long Ticks;

	public HashTuple (string oldHash, string newHash, long ticks)
	{
		Old = oldHash;
		New = newHash;
		Ticks = ticks;
	}

	public HashTuple (string oldHash, string newHash)
		: this (oldHash, newHash, DateTime.Now.AddYears (1).Ticks)
	{
	}
}
