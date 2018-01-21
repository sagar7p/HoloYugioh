using UnityEngine;
using System.Collections;

public class MonsterCard : Card {

    private double currentTime;

    private double finalTime;

    private float distance;

    new void Start()
    {
        base.Start();
        isCurrentPlayer = transform.parent.parent.parent.name.Equals(Constants.Player1);
        currentTime = 0.0;
        finalTime = 0.3f;
        distance = 0.025f;
    }

    // Update is called once per frame
    void Update()
    {
        if(monster != null)
        {
            if(currentTime > finalTime)
            {
                monster.transform.position += new Vector3(0, distance, 0);
                currentTime = 0.0f;
                distance = -distance;
            }
         
            currentTime += Time.deltaTime;
        }
        //iniatite destruction
        if (position == 0 && summoningT > 0.0f)
        {
            DestoryCard();        
        }
        //Face up attack, activate spell/trap
        if (position == 1)
        {
            CreateMonster();    
            if (aboutToFlip)
            {
                GameController.PlayAudio("Flip");
                aboutToFlip = false;
            }
            currentRotation = isCurrentPlayer ? Quaternion.Euler(-90, 0, 0) : Quaternion.Euler(-90,180,0);
            RotateToCurrentPosition();
        }
        //face down spell/trap set monster
        if (position == 2)
        {
            DestroyMonster();
            aboutToFlip = true;
            currentRotation = Quaternion.Euler(90, 90, 0);
            RotateToCurrentPosition();
        }
        //face up monster
        if (position == 3)
        {
            if (aboutToFlip)
            {
                GameController.PlayAudio("Flip");
            }
            CreateMonster();
            aboutToFlip = false;
            currentRotation = Quaternion.Euler(-90, 90, 0);
            RotateToCurrentPosition();
        }
        if (position > 0 && summoningT < 1.0f)
        {
            SummonCard();
        }

    }

    protected override void SummonCard()
    {
        if (summoningT == 0.0f)
        {
            if (position != 2)
            {
                GameController.PlayAudio("Summon");
            }
            if (position == 2)
            {
                GameController.PlayAudio("Set card");
            }
        }
        transform.rotation = currentRotation;
        var lerpedColor = Color.Lerp(Color.black, Color.white, summoningT);
        foreach (Transform child in transform)
        {
            var obj = child.gameObject;
            obj.GetComponent<Renderer>().material.SetColor("_Color", lerpedColor);
        }
        if(monster != null)
        {
            foreach (Transform child in monster.transform)
            {
                var obj = child.gameObject;
                if (obj.name.Contains("mesh"))
                    obj.GetComponent<Renderer>().material.SetColor("_Color", lerpedColor);
            }
        }
        Enable(true);
        summoningT += 0.7f * Time.deltaTime;
        if (summoningT > 1.0f)
        {
            summoningT = 1.0f;
        }
    }
}
