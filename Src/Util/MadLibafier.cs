using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class MadLibafier : Node
{

    [Export(PropertyHint.File, "*.txt")]
    public string madLibFile = "res://Assets/MadLibs/defaultLibs.txt";

    private Dictionary<string, List<MadLibReplacement>> madLibs = new Dictionary<string, List<MadLibReplacement>>();


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
        string[] splitKey = key.Split(".");
        key = splitKey[0];
        string participleRule = null; // .ing .ed .s for persentTense, pastTense, plural respectively

        if (splitKey.Length > 1)
        {
            // WARNING: This only works for the first participle rule and ignores stacking participle rules. so you cannot for example have [verb.past.present]. I mean, you can but it will only use the first participle rule.
            participleRule = splitKey[1];
        }

        if (madLibs.ContainsKey(key))
        {
            List<MadLibReplacement> values = madLibs[key];
            int index = (int)GD.RandRange(0, values.Count - 1);
            MadLibReplacement replacement = values[index];
            if (participleRule == null)
            {
                return replacement.replacement;
            }

            switch (participleRule)
            {
                case "ing":
                    return replacement.presentTenseReplacement ?? PresentTensesify(replacement.replacement);
                case "ed":
                    return replacement.pastTenseReplacement ?? PastTenstesify(replacement.replacement);
                case "s":
                    return replacement.pluralReplacement ?? Pluralize(replacement.replacement);
                default:
                    GD.PushError("Invalid participle rule: " + participleRule + " the only valid participle rules are: ing, ed, s");
                    return $"<{key}.{participleRule}>";
            }

        }
        else
        {
            return key;
        }
    }

    /// <summary>
    /// Attempts to transform the word into its present tense form using 
    /// a simple set of rules. 
    /// </summary>
    private string PresentTensesify(string word)
    {
        return word + "ing";
    }

    /// <summary>
    /// Attempts to transform the word into its past tense form using
    /// a simple set of rules.
    /// </summary>
    private string PastTenstesify(string word)
    {
        return word + "ed";
    }

    /// <summary>
    /// Attempts to transform the word into its plural form using
    /// a simple set of rules.
    /// </summary>
    private string Pluralize(string word)
    {
        return word + "s";
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
            for (int i = 0; i < values.Length; i++)
            {
                string[] data = values[i].Trim().Split("|");
                string value = values[0].Trim();
                string presentTenseValue = null; // .ing for example running
                string pastTenseValue = null;
                string pluralValue = null;

                for (int j = 0; j < data.Length; j++)
                {
                    value = data[j].Trim();
                    string[] extraData = value.Split(".");
                    string type = "invalid";
                    string replacementValue = value;
                    if (extraData.Length > 1)
                    {
                        type = extraData[0];
                        replacementValue = extraData[1];
                    }
                    switch (type)
                    {
                        case "past":
                            pastTenseValue = replacementValue;
                            break;
                        case "plural":
                            pluralValue = replacementValue;
                            break;
                        case "present":
                            presentTenseValue = replacementValue;
                            break;
                        default:
                            break;
                    }
                }

                if (value != null && value != "")
                {
                    if (madLibs.ContainsKey(key))
                    {
                        madLibs[key].Add(new MadLibReplacement(key, value, pastTenseValue, pluralValue));
                    }
                    else
                    {
                        madLibs.Add(key, new List<MadLibReplacement> { new MadLibReplacement(key, value, pastTenseValue, pluralValue) });

                    }
                }
            }
        }


    }

    private class MadLibReplacement
    {
        public string key;
        public string replacement;
        public string presentTenseReplacement; // .ing for example running
        public string pastTenseReplacement; // .ed for example ran
        public string pluralReplacement; // .s for example cats or boxes

        // TODO: Add replacement for pluralTwo and pluralMany at some point kind of how yarnspinner does it for pluralizations. Dont want to waste too much time on this right now though.

        public MadLibReplacement(string key, string replacement, string pastTenseReplacement, string pluralReplacement)
        {
            this.key = key;
            this.replacement = replacement;
            this.pastTenseReplacement = pastTenseReplacement;
            this.pluralReplacement = pluralReplacement;
        }

    }
}
