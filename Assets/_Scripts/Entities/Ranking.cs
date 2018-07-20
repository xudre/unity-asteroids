using UnityEngine;

namespace Asteroids
{
  [CreateAssetMenu(fileName = "Highscores", menuName = "Asteroids/High Score File")]
  public class Ranking : ScriptableObject
  {

    public RankingPosition[] Positions;

  }
}
