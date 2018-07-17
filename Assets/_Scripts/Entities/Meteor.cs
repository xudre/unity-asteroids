using UnityEngine;

namespace Asteroids
{
  public class Meteor : Destructible
  {

    private void OnTriggerEnter2D(Collider2D other)
    {
      if (!gameObject.activeSelf || Dead || !other.tag.Equals("PlayerBullet"))
        return;

      Life--;

      other.gameObject.SetActive(false);

      if (!Dead)
        return;

      OnDead();
    }

  }
}
