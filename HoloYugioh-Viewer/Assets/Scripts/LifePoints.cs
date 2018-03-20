using UnityEngine;
using System.Collections;

public class LifePoints : MonoBehaviour {

    public GameObject text;
    private TextMesh textMesh;
    private string initialText;
    private float totalSeconds;
    private float currentSeconds;
    private bool startSound = false;
    private int finalLifePoints;
    private int currentLifePoints;



	// Use this for initialization
	void Start () {
        textMesh = text.GetComponent<TextMesh>();
        initialText = textMesh.text.Substring(0, 7);
        currentLifePoints = 0;
        finalLifePoints = 8000;
        totalSeconds = 2.5f;
        currentSeconds = totalSeconds;
	}

    void Reset()
    {
        finalLifePoints = 8000;
        GameController.PlayAudio("Counter going up or down");
        currentSeconds = 0.0f;
        startSound = true;
    }
	
	// Update is called once per frame
	void Update () {
        UpdateTime();
	}

    void UpdateTime()
    {
        if (currentSeconds < totalSeconds)
        {
            currentSeconds += Time.deltaTime;

            int speed =  (int) ((currentLifePoints - finalLifePoints) * Time.deltaTime);

            currentLifePoints = (currentLifePoints - speed) > 0 ? currentLifePoints - speed : 0;
            UpdateText(currentLifePoints);
            
        }
        else if(startSound)
        {
            currentLifePoints = finalLifePoints;
            UpdateText(currentLifePoints);
            GameController.PlayAudio("Counter stops");
            startSound = false;
        }

    }

    void UpdateText(int lifePoints)
    {
        textMesh.text = initialText + lifePoints;
    }

    void Activate(FBData currData)
    {
        finalLifePoints = (int)currData.data.i;
        GameController.PlayAudio("Counter going up or down");
        currentSeconds = 0.0f;
        startSound = true;
        Debug.Log(finalLifePoints);
    }

}
