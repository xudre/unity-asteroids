using UnityEngine;

namespace Asteroids
{
  public class Enemy : Destructible
  {
    private void OnTriggerEnter2D(Collider2D other)
    {
      if (!gameObject.activeSelf || Dead || !other.tag.Equals("PlayerBullet"))
        return;

      Life--;

      other.gameObject.SetActive(false);

      if (!Dead)
        return;

      GameManager.Instance.Score += Points;

      gameObject.SetActive(false);
    }
  }
}
