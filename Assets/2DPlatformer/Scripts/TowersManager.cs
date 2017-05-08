using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowersManager : MonoBehaviour {
    private List<GameObject> towers;
    private System.Random rnd;
    public GameObject towerPrefab;
    public int towersNumber;
    public int maxTowerWidth;
    public int minTowerWidth;
    public int maxGapBetweenTowers;
    public int minGapBetweenTowers;
    public int pixelsToUnits;
    public float defaultYPosition;
    public float maxYVariation;
    public int startingXPosition;



    // Use this for initialization
    void Start () {
        rnd = new System.Random(Guid.NewGuid().GetHashCode());
        towers = new List<GameObject>();
        for (int i = 0; i < towersNumber; i++)
        {
            if (i==0)
            {
                float width = rnd.Next(minTowerWidth, maxTowerWidth);
                var tower = Instantiate(towerPrefab, new Vector3(startingXPosition, defaultYPosition, 0), Quaternion.identity);
                tower.transform.localScale = new Vector3(width, tower.transform.localScale.y, tower.transform.localScale.z);
                towers.Add(tower);
            }
            else
            {
                float gap = rnd.Next(minGapBetweenTowers, maxGapBetweenTowers);
                float width = rnd.Next(minTowerWidth, maxTowerWidth);
                float previousTowerWidth = towers[i - 1].transform.localScale.x;
                float x = towers[i - 1].transform.position.x + gap + width/2 + previousTowerWidth/2;
                float y = (-rnd.Next(0,1)*(rnd.Next(0,10))*maxYVariation);
                var tower = Instantiate(towerPrefab, new Vector3(x, y, 0), Quaternion.identity);
                tower.transform.localScale = new Vector3(width, tower.transform.localScale.y, tower.transform.localScale.z);
                towers.Add(tower);
            }
        }


    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
