using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HtmlSerializer
{
 

    public class Selector
    {
        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; }
        public Selector Parent { get; set; }
        public Selector Child { get; set; }

        public Selector()///
        {
            Id = "";
            TagName = "";
            Classes = new List<string>();//becase we use classes.add() must on null....
            Parent = null;
            Child = null;
        }

        public static Selector FromQueryString(string queryString)
        {
            // Split the query string by whitespace
            string[] levels = queryString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Create the root selector
            Selector rootSelector = new Selector();
            Selector currentSelector = rootSelector;

            foreach (string level in levels)
            {
                // Split the level by '#' and '.'
                string[] parts = Regex.Split(level, @"(?=[#\\.])");
                List<string> currentClasses = new List<string>();

                for (int i = 0; i < parts.Length; i++)
                {
                    if (i == 0 && !string.IsNullOrEmpty(parts[i]) && IsValidHtmlTag(parts[i]))
                    {
                        currentSelector.TagName = parts[i];
                    }
                    else if (parts[i].StartsWith("#") )
                    {
                        currentSelector.Id = parts[i].Substring(1);
                    }
                    else if (parts[i].StartsWith(".") )
                    {
                        currentSelector.Classes.Add(parts[i].Substring(1));                        
                    }
                }
                // Create a new selector for the next level
                Selector newSelector = new Selector();
                newSelector.Parent = currentSelector;
                currentSelector.Child = newSelector;
                currentSelector = newSelector;
            }
            return rootSelector;
        }
        private static bool IsValidHtmlTag(string tag)
        {
            List<string> htmlTags= HtmlHelper.Instance.HtmlTags;
            return  htmlTags.Contains(tag);
        }
    }
}

