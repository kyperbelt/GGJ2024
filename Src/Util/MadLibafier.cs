using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class MadLibafier : Node
{

    [Export(PropertyHint.File, "*.txt")]
    public string madLibFile = "res://Assets/MadLibs/defaultLibs.txt";

    private Dictionary<string, List<string>> madLibs = new Dictionary<string, List<string>>();


    public override void _Ready()
    {
        AddToGroup("MadLibafier");
        LoadMapLibFile();
    }

    /// <summary>
    /// returns a string with all the keys replaced with random values from the madLibs dictionary
    /// </summary>
    public string GetMadLib(string libString)
    {
        string newString = libString;
        int start = newString.IndexOf("[");
        int end = newString.IndexOf("]");
        while (start != -1 && end != -1)
        {
            string key = newString.Substring(start + 1, end - start - 1);
            string replacement = GetRandomReplacement(key);
            if (replacement == key)
            {
                GD.Print("MadLibafier: key not found in madLibs dictionary: " + key);
                replacement = "<" + key + ">";
            }
            newString = newString.Replace("[" + key + "]", replacement);
            start = newString.IndexOf("[");
            end = newString.IndexOf("]");
        }
        return newString;
    }

    /// <summary>
    /// get a random replacement word from the madLibs dictionary
    /// if the key is not found in the dictionary return the key
    /// </summary>
    private string GetRandomReplacement(string key)
    {
        if (madLibs.ContainsKey(key))
        {
            List<string> values = madLibs[key];
            int index = (int)GD.RandRange(0, values.Count);
            return values[index];
        }
        else
        {
            return key;
        }
    }

    private void LoadMapLibFile()
    {
        // load the madLibFile into the madLibs dictionary
        // each line in the file starts with a key: followed by a list of comma separated values
        // the key is the word to be replaced in the madlib 
        // the values are the words that can be used to replace the key 
        // example: 
        //	noun: dog, cat, mouse, house
        //	verb: run, jump, skip, hop 
        // But we are not limited to only these simple replacements. For example we can have complex replacements like this: 
        //      setupLine1: Did you [hear] about the [adjective] [noun] who [verb] a [adjective] [noun]?, There once was a [adjective] [noun] who [verb] a [adjective] [noun].
        //      This will recursively resolve the madlib until it hits a base case and return the final string.

        if (madLibFile == null || madLibFile == "")
        {
            GD.Print("No madLibFile specified");
            return;
        }

        if (!FileAccess.FileExists(madLibFile))
        {
            GD.Print("madLibFile does not exist");
            return;
        }

        FileAccess file = FileAccess.Open(madLibFile, FileAccess.ModeFlags.Read);

        while (!file.EofReached())
        {
            string line = file.GetLine();

            if (line == null || line == "")
            {
                continue;
            }
            string[] splitLine = line.Split(":");
            string key = splitLine[0];
            string[] values = splitLine[1].Split(",");
            madLibs.Add(key, new List<string>(values));
        }

    }
}
