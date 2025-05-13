using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SquirrelBombMod.Spamton
{
    public class NeoAnimatedPortrait : ManagedBehaviour
    {
        public void Start()
        {
            //transform.Find("Anim").Find("Body").Find("Head").GetComponent<Animator>().speed = 1.25f;
        }

        public void AttackSpot()
        {
            transform.Find("Anim").Find("Body").Find("Head").GetComponent<Animator>().SetTrigger("shoot");
        }

        public void OpenMouth()
        {
            transform.Find("Anim").Find("Body").Find("Head").GetComponent<Animator>().SetTrigger("openMouth");
        }

        public void CloseMouth()
        {
            transform.Find("Anim").Find("Body").Find("Head").GetComponent<Animator>().SetTrigger("closeMouth");
        }

        public Material GetGlowyMaterial()
        {
            return transform.Find("Anim").Find("Body").Find("Head").Find("mouth").Find("chargeflash").GetComponent<Renderer>().material;
        }
    }
}
