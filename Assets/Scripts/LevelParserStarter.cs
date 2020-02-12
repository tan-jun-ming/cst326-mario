using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelParserStarter : MonoBehaviour
{
    public string filename;

    public GameObject Rock;

    public GameObject Brick;

    public GameObject CoinBox;

    public GameObject Stone;

    public Transform parentTransform;
    // Start is called before the first frame update
    void Start()
    {
        RefreshParse();
    }


    private void FileParser()
    {
        string fileToParse = string.Format("{0}{1}{2}.txt", Application.dataPath, "/Resources/", filename);

        using (StreamReader sr = new StreamReader(fileToParse))
        {
            string line = "";
            int column = 19;

            while ((line = sr.ReadLine()) != null)
            {
                
                int row = 0;
                char[] letters = line.ToCharArray();
                foreach (var letter in letters)
                {
                    SpawnPrefab(letter, new Vector3(-row, column, 0));
                    row++;
                }
                column--;
            }

            sr.Close();
        }
    }

    private void SpawnPrefab(char spot, Vector3 positionToSpawn)
    {
        GameObject ToSpawn = null;
        int thickness = 0;

        switch (spot)
        {
            case 'b': ToSpawn = Brick; break;
            case '?': ToSpawn = CoinBox; break;
            case 'x': ToSpawn = Rock; thickness = 1; break;
            case 's': ToSpawn = Stone; thickness = 1; break;
            //default: Debug.Log("Default Entered"); break;
        }

        if (ToSpawn != null)
        {
            int thickness_back = (int)positionToSpawn.z - thickness;
            int thickness_front = (int)positionToSpawn.z + thickness;

            for (int i = thickness_back; i <= thickness_front; i++)
            {
                ToSpawn.transform.localPosition = positionToSpawn + Vector3.forward * i;
                ToSpawn = GameObject.Instantiate(ToSpawn, parentTransform);
            }

        }
    }

    public void RefreshParse()
    {
        GameObject newParent = new GameObject();
        newParent.name = "Environment";
        newParent.transform.position = gameObject.transform.position;
        newParent.transform.parent = this.transform;
        
        if (parentTransform) Destroy(parentTransform.gameObject);

        parentTransform = newParent.transform;
        FileParser();
    }
}
