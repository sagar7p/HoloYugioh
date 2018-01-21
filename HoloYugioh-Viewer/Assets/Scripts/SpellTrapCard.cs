using UnityEngine;
using System.Collections;

public class SpellTrapCard : Card {

    new void Start()
    {
        base.Start();
        var grandparent = transform.parent.parent;
        if(grandparent != null)
        {
            isCurrentPlayer = grandparent.name.Equals(Constants.Player1) || grandparent.parent.name.Equals(Constants.Player1);
        }
        else
        {
            isCurrentPlayer = true;
        }
        
    }

    // Update is called once per frame
    void Update()
     {
         //iniatite destruction
         if (position == 0 && summoningT > 0.0f)
         {
             DestoryCard();
         }
         //Face up attack, activate spell/trap
         if (position == 1)
         {
            if (aboutToFlip)
            {
                GameController.PlayAudio("Spell Activate");
                aboutToFlip = false;
            }
             currentRotation = isCurrentPlayer ? Quaternion.Euler(-90, 0, 0) : Quaternion.Euler(-90,180,0);
             RotateToCurrentPosition();
         }
         //face down spell/trap
         if (position == 2)
         {
             aboutToFlip = true;
             currentRotation = Quaternion.Euler(90, 0, 0);
             RotateToCurrentPosition();
         }
         if (position > 0 && summoningT < totalTime)
         {
             SummonCard();
         }

     }


     protected override void DestoryCard()
     {
         aboutToFlip = false;
         var lerpedColor = Color.Lerp(Color.black, Color.white, summoningT);
         foreach (Transform child in transform)
         {
             var obj = child.gameObject;
             obj.GetComponent<Renderer>().material.SetColor("_Color", lerpedColor);
         }
         summoningT -= 0.7f * Time.deltaTime;
         if (summoningT < 0.0f)
         {
             summoningT = 0.0f;
             Enable(false);
         }
     }
}
