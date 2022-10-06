using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MuscleGroups : MonoBehaviour
{

    public static MuscleGroups Instance;

    public TextAsset[] texts;

    private List<string>[] groups;
    Dictionary<string, List<Visibility>[]> insertions = new Dictionary<string, List<Visibility>[]>();
    Dictionary<string, Visibility> muscles = new Dictionary<string, Visibility>();

    private void Awake()
    {
        Instance = this;

        groups = new List<string>[texts.Length];

        for (int i = 0; i < texts.Length; i++)
        {
            groups[i] = new List<string>();
            foreach (var line in texts[i].text.Split("\n"))
            {
                if(line.Length > 0)
                    groups[i].Add(line);
            }
        }

        var insertionsList = GlobalVariables.Instance.globalParent.GetComponentsInChildren<Visibility>(true).Where(it => it.CompareTag("Insertions"));
        var musclesList = GlobalVariables.Instance.globalParent.GetComponentsInChildren<Visibility>(true).Where(it => it.CompareTag("Muscles"));


        // Index 0 = Right insertion
        // Index 1 = Left insertion
        foreach (var item in insertionsList)
        {
            if(!insertions.ContainsKey(item.nameScript.originalName.RemoveSuffix()))
                insertions.Add(item.nameScript.originalName.RemoveSuffix(), new List<Visibility>[2]);


            if (insertions[item.nameScript.originalName.RemoveSuffix()][0] == null)
                insertions[item.nameScript.originalName.RemoveSuffix()][0] = new List<Visibility>();

            if (insertions[item.nameScript.originalName.RemoveSuffix()][1] == null)
                insertions[item.nameScript.originalName.RemoveSuffix()][1] = new List<Visibility>();

            if (item.nameScript.originalName.IsRight())
                insertions[item.nameScript.originalName.RemoveSuffix()][0].Add(item);
            else
                insertions[item.nameScript.originalName.RemoveSuffix()][1].Add(item);
            
        }

        foreach (var item in musclesList)
            muscles.Add(item.nameScript.originalName, item);
    }

    //Get all the muscles of this insertion
    public List<Visibility> GetGroupMuscles(Visibility insertion)
    {
        List<Visibility> allGroupMuscles = new List<Visibility>();

        var insertionName = insertion.nameScript.originalName.RemoveSuffix();

        string suffix = "";

        if(insertion.nameScript.originalName.IsRight())
            suffix = ".r";
        else if(insertion.nameScript.originalName.IsLeft())
            suffix = ".l";

        foreach (var group in groups)
        {
            if(group.Contains(insertionName))
            {
                for (int i = 0; i < group.Count; i++)
                {
                    string muscleName = group[i] + suffix;
                    if(muscles.ContainsKey(muscleName))
                        allGroupMuscles.Add(muscles[muscleName]);
                    /*else
                        allGroupMuscles.Add(muscles[group[i] + ".g"]);*/

                }
            }
        }

        return allGroupMuscles;
    }

    //Get all the muscles that converges in the insertion
    public void GetConvergingMuscles(Visibility insertion)
    {
        var insertionName = insertion.nameScript.originalName.RemoveSuffix();

        string suffix = "";

        if (insertion.nameScript.originalName.IsRight())
            suffix = ".r";
        else if (insertion.nameScript.originalName.IsLeft())
            suffix = ".l";

        foreach (var group in groups)
        {
            if (insertionName == group[0])
            {
                for (int i = 1; i < group.Count; i++)
                {
                    string muscleName = group[i] + suffix;
                    if (muscles.ContainsKey(muscleName))
                        insertion.insertionMuscles.Add(muscles[muscleName].GetComponent<BodyPart>());
                }
            }
        }
    }
    public void GetGroupInsertions(Visibility muscle)
    {
        var muscleName = muscle.nameScript.originalName.RemoveSuffix();

        bool isRight = muscle.nameScript.originalName.IsRight();

        foreach (var group in groups)
        {
            if (group.Contains(muscleName))
            {
                for (int i = 0; i < group.Count; i++)
                {
                    if (!insertions.ContainsKey(group[i]))
                        continue;

                    if(isRight && insertions[group[i]][0] != null)
                        muscle.insertions.AddRange(insertions[group[i]][0].ConvertAll(it => it.GetComponent<BodyPart>()));
                    else if(insertions[group[i]][1] != null)
                        muscle.insertions.AddRange(insertions[group[i]][1].ConvertAll(it => it.GetComponent<BodyPart>()));
                    
                }
            }
        }
    }




}
