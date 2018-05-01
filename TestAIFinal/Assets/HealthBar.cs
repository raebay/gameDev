using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {

    //public Image currentHealthBar;
    //public Text rationText;

    private float hitpoints = 150;
    private float maxHitPoints = 150;

	// Use this for initialization
	void Start () {
    //    UpdateHealthbar();
	}

    private void UpdateHealthbar() {
//        float ratio = hitpoints / maxHitPoints;
  //      currentHealthBar.rectTransform.localScale = new Vector3(ratio, 1, 1);
    }

    private void TakeDamage(float damage) {
        hitpoints -= damage;
        print(hitpoints);
        if (hitpoints < 0) {
            hitpoints = 0;
            Debug.Log("Dead!");
        }
    }

    private void HealDamage(float heal)
    {
        hitpoints += heal;
        if (hitpoints > maxHitPoints)
        {
            hitpoints = maxHitPoints;
            Debug.Log("Healed!!");
        }
    }
}
