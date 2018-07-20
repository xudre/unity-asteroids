using System;

namespace Asteroids
{
  [Serializable]
  public struct RankingPosition
  {
    public string Name;
    public int Score;

    public RankingPosition(string name, int score)
    {
      Name = name;
      Score = score;
    }

  }
}