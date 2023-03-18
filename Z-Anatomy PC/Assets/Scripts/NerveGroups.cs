using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NerveGroups : MonoBehaviour
{
    public static NerveGroups Instance;

    public TextAsset[] texts;

    private List<string>[] groups;
    Dictionary<string, List<BodyPartVisibility>[]> nerves = new Dictionary<string, List<BodyPartVisibility>[]>();
    Dictionary<string, List<BodyPartVisibility>[]> muscles = new Dictionary<string, List<BodyPartVisibility>[]>();
    private void Awake()
    {
        Instance = this;

        groups = new List<string>[texts.Length];

        for (int i = 0; i < texts.Length; i++)
        {
            groups[i] = new List<string>();
            foreach (var line in texts[i].text.Split("\n"))
            {
                if (line.Length > 0)
                    groups[i].Add(line);
            }
        }

        var nervesList = GlobalVariables.Instance.globalParent.GetComponentsInChildren<BodyPartVisibility>(true).Where(it => it.CompareTag("Nervous"));
        var musclesList = GlobalVariables.Instance.globalParent.GetComponentsInChildren<BodyPartVisibility>(true).Where(it => it.CompareTag("Muscles"));


        // Index 0 = Right insertion
        // Index 1 = Left insertion
        foreach (var nerve in nervesList)
        {
            if (!nerves.ContainsKey(nerve.nameScript.originalName.RemoveSuffix()))
                nerves.Add(nerve.nameScript.originalName.RemoveSuffix(), new List<BodyPartVisibility>[2]);


            if (nerves[nerve.nameScript.originalName.RemoveSuffix()][0] == null)
                nerves[nerve.nameScript.originalName.RemoveSuffix()][0] = new List<BodyPartVisibility>();

            if (nerves[nerve.nameScript.originalName.RemoveSuffix()][1] == null)
                nerves[nerve.nameScript.originalName.RemoveSuffix()][1] = new List<BodyPartVisibility>();

            if (nerve.nameScript.originalName.IsRight())
                nerves[nerve.nameScript.originalName.RemoveSuffix()][0].Add(nerve);
            else
                nerves[nerve.nameScript.originalName.RemoveSuffix()][1].Add(nerve);
        }

        foreach (var muscle in musclesList)
        {
            if (!muscles.ContainsKey(muscle.nameScript.originalName.RemoveSuffix()))
                muscles.Add(muscle.nameScript.originalName.RemoveSuffix(), new List<BodyPartVisibility>[2]);


            if (muscles[muscle.nameScript.originalName.RemoveSuffix()][0] == null)
                muscles[muscle.nameScript.originalName.RemoveSuffix()][0] = new List<BodyPartVisibility>();

            if (muscles[muscle.nameScript.originalName.RemoveSuffix()][1] == null)
                muscles[muscle.nameScript.originalName.RemoveSuffix()][1] = new List<BodyPartVisibility>();

            if (muscle.nameScript.originalName.IsRight())
                muscles[muscle.nameScript.originalName.RemoveSuffix()][0].Add(muscle);
            else
                muscles[muscle.nameScript.originalName.RemoveSuffix()][1].Add(muscle);
        }
    }

    //Get all the muscles of this nerve
    public void GetNerveMuscles(BodyPartVisibility nerve)
    {
        var nerveName = nerve.nameScript.originalName.RemoveSuffix();

        bool isRight = nerve.nameScript.originalName.IsRight();

        foreach (var group in groups)
        {
            if (group.Contains(nerveName))
            {
                for (int i = 0; i < group.Count; i++)
                {
                    if (group[i].ToLower().Contains("nerve"))
                        continue;

                    if (isRight && muscles[group[i]][0] != null)
                        nerve.nerveMuscles.AddRange(muscles[group[i]][0].ConvertAll(it => it.GetComponent<TangibleBodyPart>()));
                    else if (muscles[group[i]][1] != null)
                        nerve.nerveMuscles.AddRange(muscles[group[i]][1].ConvertAll(it => it.GetComponent<TangibleBodyPart>()));

                }
            }
        }
    }

    public void GetMuscleNerves(BodyPartVisibility muscle)
    {
        var muscleName = muscle.nameScript.originalName.RemoveSuffix();

        bool isRight = muscle.nameScript.originalName.IsRight();

        foreach (var group in groups)
        {
            if (group.Contains(muscleName))
            {
                for (int i = 0; i < group.Count; i++)
                {
                    if (!group[i].ToLower().Contains("nerve"))
                        continue;

                    if (isRight && nerves[group[i]][0] != null)
                        muscle.muscleNerves.AddRange(nerves[group[i]][0].ConvertAll(it => it.GetComponent<TangibleBodyPart>()));
                    else if (nerves[group[i]][1] != null)
                        muscle.muscleNerves.AddRange(nerves[group[i]][1].ConvertAll(it => it.GetComponent<TangibleBodyPart>()));

                }
            }
        }
    }
}
