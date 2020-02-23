using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelParser : MonoBehaviour
{
    public string filename;

    public GameObject Rock;
    public GameObject Brick;
    public GameObject Coin;
    public GameObject CoinBox;
    public GameObject Stone;
    public GameObject Water;
    public GameObject Background;
    public GameObject Pole;

    public Transform parentTransform;

    private List<bool> solidity = new List<bool>();
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
            int row = 19;

            while ((line = sr.ReadLine()) != null)
            {
                
                int column = 0;
                char[] letters = line.ToCharArray();
                foreach (var letter in letters)
                {
                    SpawnPrefab(letter, new Vector3(-column, row, 0));
                    column++;
                }
                row--;
            }

            sr.Close();
        }
    }

    private void SpawnPrefab(char spot, Vector3 positionToSpawn)
    {
        GameObject ToSpawn = null;
        int thickness = 0;
        bool solid = true;

        int col = (int)Mathf.Abs(positionToSpawn.x);

        //print(solidity.Count);
        while (solidity.Count <= col)
        {
            solidity.Add(false);
        }
        bool solid_above = solidity[col];

        switch (spot)
        {
            case 'b': ToSpawn = Brick; break;
            case 'c': ToSpawn = Coin; solid = false; break;
            case '?': ToSpawn = CoinBox; break;
            case 'x': ToSpawn = Rock; thickness = 1; break;
            case 's': ToSpawn = Stone; thickness = 1; break;
            case 'w': ToSpawn = Water; thickness = 1; break;
            case 'd': ToSpawn = Background; solid = false; break;
            case 'p': ToSpawn = Pole; solid = false; break;
            default: solid = false; break;
        }

        if (ToSpawn != null)
        {
            int thickness_back = (int)positionToSpawn.z - thickness;
            int thickness_front = (int)positionToSpawn.z + thickness;

            for (int i = thickness_back; i <= thickness_front; i++)
            {
                ToSpawn.transform.localPosition = positionToSpawn + Vector3.forward * i;
                GameObject new_obj = GameObject.Instantiate(ToSpawn, parentTransform);

                if (solidity[col] && spot == "w"[0])
                {
                    ParticleSystem ps = ((ParticleSystem)new_obj.transform.GetChild(0).GetComponent(typeof(ParticleSystem)));

                    ParticleSystem.EmissionModule em = ps.emission;
                    em.enabled = false;

                }
            }

        }

        solidity[col] = solid;
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
