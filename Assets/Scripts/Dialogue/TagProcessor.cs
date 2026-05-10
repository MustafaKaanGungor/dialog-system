using System.Collections.Generic;
using UnityEngine;

namespace DialogSystem.Dialogue
{
    public static class TagProcessor
    {
        public struct TagCommand
        {
            public int CharIndex;
            public string TagName;
            public string Value;
        }
        public struct ParseResult
        {
            public string CleanText;
            public List<TagCommand> Commands;
        }

        public static ParseResult Parse(string text)
        {
            string[] subTexts = text.Split('<', '>');
            string cleanText = "";

            ParseResult result = new ParseResult();
            result.Commands = new List<TagCommand>();

            for(int i = 0; i < subTexts.Length; i++)
            {
                if(i % 2 == 0)
                {
                    cleanText += subTexts[i];
                } else if(!IsCustomTag(subTexts[i].Replace(" ", "")))
                {
                    cleanText += $"<{subTexts[i]}>";
                } else
                {
                    string[] commandText = subTexts[i].Split('=');
                    TagCommand command = new TagCommand()
                    {
                        CharIndex = cleanText.Length,
                        TagName = commandText[0].Trim().ToLower(),
                        Value = commandText.Length > 1 ? commandText[1].Trim().ToLower() : null
                    };

                    result.Commands.Add(command);
                }
            }

            result.CleanText = cleanText;

            return result;
        }

        private static bool IsCustomTag(string tag)
        {
            return tag.StartsWith("speed=") || tag.StartsWith("pause=") || tag.StartsWith("emotion=") || tag.StartsWith("action=");
        }
    }
}

