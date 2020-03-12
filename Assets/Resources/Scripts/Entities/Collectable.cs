using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.localEulerAngles = new Vector3(0, (360 * Time.time)%360, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Destroy(gameObject);
            Game.game.PlayNextSound(Game.game.sound_fx_coin);
            Game.data.coins ++;
        }
    }
}
