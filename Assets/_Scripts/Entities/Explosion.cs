using UnityEngine;

namespace Asteroids
{
  public class Explosion : MonoBehaviour {

    public void OnAnimationEnd()
    {
      DestroyObject(gameObject);
    }

  }
}

