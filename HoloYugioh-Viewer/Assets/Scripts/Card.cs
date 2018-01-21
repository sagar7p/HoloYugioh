using UnityEngine;
using System.Collections;

public class Card : MonoBehaviour {

    //renderer
    protected Renderer render;

    //card details
    protected int position;
    protected string cardName;

    //roations
    protected Quaternion currentRotation;
    protected float summoningT;
    protected float totalTime;

    //sound
    protected bool aboutToFlip;

    //FBData
    protected FBData currData;

    protected GameObject monster;

    //names
    protected bool isCurrentPlayer;

    // Use this for initialization
    protected void Start () {
        position = -1;
        cardName = "";
        summoningT = 0.0f;
        totalTime = 1.0f;
        aboutToFlip = false;
        Enable(false);
	}

    //Spell Trap Field Default
    protected virtual void SummonCard()
    {
        if (summoningT == 0.0f)
        {
            if (position == 1)
            {
                GameController.PlayAudio("Spell Activate");
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
        Enable(true);
        summoningT += 0.7f * Time.deltaTime;
        if (summoningT > totalTime)
        {
            summoningT = totalTime;
        }
    }

    //Monster  Default
    protected virtual void DestoryCard()
    {     
         aboutToFlip = false;
         if (summoningT == 1.0f)
         {
            GameController.PlayAudio("Destroyed");
            foreach (Transform child in transform)
            {
                var obj = child.gameObject;
                obj.AddComponent<TriangleExplosion>();

                StartCoroutine(obj.GetComponent<TriangleExplosion>().SplitMesh(false));
            }
            if(monster != null)
            {
                monster.AddComponent<TriangleExplosion>();
                StartCoroutine(monster.GetComponent<TriangleExplosion>().SplitMesh(true));
            }
         }
        /* if(monster != null)
         {
           
            var lerpedColor = Color.Lerp(Color.black, Color.white, summoningT);
            foreach (Transform child in monster.transform)
            {
                var obj = child.gameObject;
                if(obj.name.Contains("mesh"))
                    obj.GetComponent<Renderer>().material.SetColor("_Color", lerpedColor);
            }
        }*/
        summoningT -= 0.7f * Time.deltaTime;
        if (summoningT < 0.0f)
        {
            summoningT = 0.0f;
            Enable(false);
            //DestroyMonster();
        }

    }


    protected void RotateToCurrentPosition()
    {
        float speed = 2.0f;
        transform.rotation = Quaternion.Slerp(transform.rotation, currentRotation, speed * Time.deltaTime);
    }

    protected void Enable(bool enable)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.GetComponent<Renderer>().enabled = enable;
        }
    }

    protected virtual void Activate(FBData currData)
    {
        if (currData.data.IsString)
        {
            cardName = currData.data.str;
            StartCoroutine(DrawCard(cardName));
        }
        else
        {
            position = (int)currData.data.i;
        }
    }

    protected IEnumerator DrawCard(string name)
    {
        if(!string.IsNullOrEmpty(name))
        {
            string endpoint = "https://yugiohprices.com/api/card_image/";
            name = name.Replace(' ', '_');
            var link = endpoint + name;
            if (name == "back")
            {
                link = "http://vignette1.wikia.nocookie.net/yugioh/images/e/e5/Back-EN.png/revision/latest?cb=20100726082133";
            }
            Texture2D tex;
            tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
            WWW www = new WWW(link);
            yield return www;
            www.LoadImageIntoTexture(tex);

            transform.Find("Front").gameObject.GetComponent<Renderer>().material.mainTexture = tex;
        }
       
    }

    public void Reset()
    {
        position = 0;
        cardName = "";
    }

    protected IEnumerator DrawMonster(string name, GameObject obj)
    {
        if (!string.IsNullOrEmpty(name))
        {
            string endpoint = "https://yugiohprices.com/api/card_image/";
            name = name.Replace(' ', '_');
            var link = endpoint + name;
            if (name == "back")
            {
                link = "http://vignette1.wikia.nocookie.net/yugioh/images/e/e5/Back-EN.png/revision/latest?cb=20100726082133";
            }
            Texture2D tex;
            tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
            WWW www = new WWW(link);
            yield return www;
            www.LoadImageIntoTexture(tex);
            var newTex = cropTexture(new Rect(40, tex.height * 0.30f, tex.width - 80, tex.width - 100), tex);
            obj.GetComponent<Renderer>().material.mainTexture = newTex;
        }

    }

    private Texture2D cropTexture(Rect sourceRect, Texture2D sourceTex)
    {
        int x = Mathf.FloorToInt(sourceRect.x);
        int y = Mathf.FloorToInt(sourceRect.y);
        int width = Mathf.FloorToInt(sourceRect.width);
        int height = Mathf.FloorToInt(sourceRect.height);

        Color[] pix = sourceTex.GetPixels(x, y, width, height);
        Texture2D destTex = new Texture2D(width, height);
        destTex.SetPixels(pix);
        destTex.Apply();

        return destTex;
    }

    public void CreateMonster()
    {
        if(monster == null)
        {
            monster = Instantiate(Resources.Load("Monster/Monster")) as GameObject;
            monster.transform.localScale *= 0.7f;
            monster.transform.localEulerAngles = new Vector3(0, 90, 0);
            if (!isCurrentPlayer)
            {
                monster.transform.localEulerAngles = new Vector3(0, 180, 0);
            }
            monster.transform.position = this.transform.position + new Vector3(0, monster.transform.localScale.x, 0);
            StartCoroutine(DrawMonster(cardName,monster));
        }   
    }

    public void DestroyMonster()
    {
        Destroy(monster);
        monster = null;      
    }

}
