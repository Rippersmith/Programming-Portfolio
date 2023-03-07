using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    public ParticleSystem explosion1, explosion2;

    private void Start()
    {
        explosion1.Stop();
        explosion2.Stop();
    }

    //get the colors on the moon overlays, then set off the explosions on that planet with those colors
    public void GetColorsAndPlayExplosion(Vector3 moveToPoint, Color baseMoonColor, Color overlayMoonColor)
    {
        var ex1Main = explosion1.main;
        var ex2Main = explosion2.main;

        transform.position = moveToPoint;

        ex1Main.startColor = baseMoonColor;
        ex2Main.startColor = overlayMoonColor;

        explosion1.Play();
        explosion2.Play();
    }
}
