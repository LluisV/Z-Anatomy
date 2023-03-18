using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using System.Threading.Tasks;
using System.Diagnostics;
using System;
using System.Text;
using System.Text.RegularExpressions;

public class HighlightText : MonoBehaviour
{

    public static HashSet<string> bodypartsHyperlinks;

    public static void GetTranslatedNames()
    {
        bodypartsHyperlinks = new HashSet<string>();


        foreach (var nameScript in GlobalVariables.Instance.allNameScripts)
        {
            bodypartsHyperlinks.Add(nameScript.name.Replace("(R)", "").Replace("(L)", "").Trim().ToLower());

            if (nameScript.HasSynonims())
            {
                foreach (var synonym in nameScript.allSynonyms[Settings.languageIndex])
                {
                    bodypartsHyperlinks.Add(synonym.ToLower());
                }
            }
        }

        bodypartsHyperlinks.OrderBy(it => it);
    }

    //It hightlights other body parts in the text
    public static IEnumerator Hightlight(string text, string name, TMP_InputField inputField)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        string originalDesc = text;

        //FindAndHighlightLinkMatches(tmpro, ref originalDesc, name);

        string toLowerDesc = originalDesc.ToLower();

        HashSet<string> words = toLowerDesc.RemovePunctuations().Split().ToHashSet();
        HashSet<string> matches = new HashSet<string>();

        UnityEngine.Debug.Log("Count: " + bodypartsHyperlinks.Count);

        //Find matches
        foreach (string hyperlink in bodypartsHyperlinks)
        {
            var split = hyperlink.Split(' ');
            bool find = false;
            if(hyperlink != name)
            {
                for (int i = 0; i < split.Length; i++)
                {
                    find = words.Contains(split[i]);
                    if (!find)
                        break;
                }
                if(find && hyperlink.Length > 0)
                    matches.Add(hyperlink);
                find = false;
            }
        }

        int indexSpacing = 0;

        string[] lines = text.Split(new string[] { "\r", "\n" }, StringSplitOptions.None);
        int[] indexs = new int[lines.Length];
        int index = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            indexs[i] = index;
            index += lines[i].Length + 1;
        }

        index = 0;

        //Foreach line in text
        foreach (var line in lines)
        {
            try
            {
                List<string> lineMatches = new List<string>();

                foreach (var match in matches)
                    if (line.Contains(match))
                        lineMatches.Add(match);

                //Get bigger match
                foreach (var match in lineMatches.ToList())
                {
                    List<string> duplicates = lineMatches.Where(it => it.Contains(match)).OrderBy(it => it.Length).ToList();

                    //If this match is the smallest one
                    if (duplicates.Count() > 1 && match == duplicates[0])
                        lineMatches.Remove(match);
                }

                //Order by left to right
                lineMatches = lineMatches.OrderBy(it => line.IndexOf(it)).ToList();


                int firstWordIndex = 0;
                int lastWordIndex = 0;

                //Foreach match in this line
                foreach (string match in lineMatches)
                {
                    int indexOfMatch = line.IndexOf(match);
                    if (lastWordIndex > indexOfMatch)
                        continue;

                    //Get the match index
                    if (indexOfMatch == 0)
                        firstWordIndex = 0;
                    else
                        firstWordIndex = GetFirstWordIndex(line, indexOfMatch) + 1;
                    lastWordIndex = GetLastWordIndex(line, indexOfMatch + match.Length - 1);
                    string completeWord = toLowerDesc.Substring(indexs[index] + firstWordIndex, lastWordIndex - firstWordIndex);

                    //Get real index
                    int realIndex = indexs[index] + indexOfMatch;

                    //If there's a >2 char difference between matches, don't highlight it
                    if (completeWord.Length - match.Length <= 2)
                    {
                        //Set color
                        originalDesc = originalDesc.Insert(realIndex + indexSpacing, new StringBuilder().Append("<link><color=#").Append(ColorUtility.ToHtmlStringRGB(GlobalVariables.HighligthColor)).Append("><b>").ToString());
                        indexSpacing += 24;
                        originalDesc = originalDesc.Insert(realIndex + match.Length + indexSpacing, "</b></link></color>");
                        indexSpacing += 19;
                    }
                }
            }
            catch (Exception e) 
            {
                UnityEngine.Debug.LogError(e + "      \n" + "line: " +  line);
            }
            index++;
        }

        stopwatch.Stop();
        UnityEngine.Debug.Log("Highlight: " + (float)(stopwatch.ElapsedMilliseconds / 1000f));


        // stopwatch.Restart();

        inputField.text = originalDesc;

        yield return null;

        //stopwatch.Stop();
        // UnityEngine.Debug.Log("Set text: " + (float)(stopwatch.ElapsedMilliseconds / 1000f) + " characters: " + originalDesc.Length);

    }

    private static void FindAndHighlightLinkMatches(string[] links, int[] indexes, ref string originalDesc)
    {
        int indexSpacing = 0;
        int index = 0;

        //Foreach line in text
        foreach (var line in links)
        {
            try
            {
                //Get real index
                int realIndex = indexes[index];
                int lastWordIndex = GetLastWordIndex(originalDesc, realIndex + line.Length - 1 + indexSpacing);

                //Set color
                originalDesc = originalDesc.Insert(realIndex + indexSpacing, "<link><color=#" 
                    + ColorUtility.ToHtmlStringRGB(GlobalVariables.HighligthColor) + ">");
                indexSpacing += 21;

                if (lastWordIndex == originalDesc.Length)
                    originalDesc += "</link></color>";
                else
                {
                    originalDesc = originalDesc.Insert(lastWordIndex + indexSpacing, "</link></color>");
                    indexSpacing += 15;
                }
                
            }
            catch { }
            index++;
        }
    }



    private static int GetFirstWordIndex(string line, int index)
    {
        char c = line[index];
        bool startOfWord = c == ' ' || c == '.' || c == ',' || c == '\n' || c == '>' || c == ';' || c == '(';

        while (!startOfWord)
        {
            try
            {
                c = line[index];
                startOfWord = c == ' ' || c == '.' || c == ',' || c == '\n' || c == '>' || c == ';' || c == '(';
                if (!startOfWord)
                    index--;
            }
            catch
            {
                return index + 1;
            }

        }
        return index;
    }

    private static int GetLastWordIndex(string line, int index)
    {
        char c = line[index];
        bool endOfWord = c == ' ' || c == '.' || c == ',' || c == '\n' || c == '<' || c == ';' || c == ')';
        while (!endOfWord)
        {
            c = line[index];
            endOfWord = c == ' ' || c == '.' || c == ',' || c == '\n' || c == '<' || c == ';' || c == ')';
            if (!endOfWord)
                index++;
            if (index == line.Length)
                return line.Length;
        }

        return index;
    }


}
