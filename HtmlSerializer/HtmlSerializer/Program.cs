using HtmlSerializer;
using Microsoft.VisualBasic;
using System;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

//async void SearchInHtml()
{
    //load and clean
    Console.WriteLine("enter link of html");
    var url = Console.ReadLine();
    var html = await Load(url);
    var cleanHtml = SplitAndCleanHtml(html);

    //html serializer
    var HtmlTags = HtmlHelper.Instance.HtmlTags;
    var HtmlVoidTags = HtmlHelper.Instance.HtmlVoidTags;

    HtmlElement root = BuildTreehtml(cleanHtml, HtmlTags, HtmlVoidTags);


    //enter quary to find 
    Console.WriteLine("enter quary");
    var quary = Console.ReadLine();
    var selector = Selector.FromQueryString(quary);

    var listHtmlSuitable = HtmlElementExtensions.FindBySelector(root, selector).ToList();

    //print 
    Console.WriteLine($"we found {listHtmlSuitable.Count}");
    listHtmlSuitable.ForEach(l => Console.WriteLine(l));
 }






    IEnumerable<string> SplitAndCleanHtml(string html)
    {
        var splitHtml = new Regex("<(.*?)>").Split(html);
        var regaxCleanHtml = new Regex("[\\t\\n\\r\\v\\f]");
        var cleanHtml = splitHtml.Select(line => regaxCleanHtml.Replace(line, "")).Where(l => !string.IsNullOrWhiteSpace(l));
        return cleanHtml;
    }

    async Task<string> Load(string url)
    {
        HttpClient client = new HttpClient();
        var response = await client.GetAsync(url);
        var html = await response.Content.ReadAsStringAsync();
        return html;
    }

    HtmlElement BuildTreehtml(IEnumerable<string> htmlLines, List<string> htmlTags, List<string> htmlVoidTags)
    {
        var root = new HtmlElement
        {
            Name = "html",
        };

        var elementsStack = new Stack<HtmlElement>();
        elementsStack.Push(root);

        foreach (var line in htmlLines)
        {
            var trimmedLine = line.Trim();

            // בדיקה אם מדובר בתגית "html/" (סוף)
            if (trimmedLine.Equals("/html"))
            {
                break;
            }

            // בדיקה אם מדובר בתגית סוגרת
            if (trimmedLine.StartsWith("/"))
            {
                var closedElement = elementsStack.Pop();

                // אם מדובר בתגית סוגרת, התוכן הפנימי נשמר
                closedElement.InnerHtml = closedElement.InnerHtml.Trim();
                continue;
            }
            // שליפת שם התגית
            var tagName = trimmedLine.Split(' ')[0];

            // אם התגית מוכרת (html או void)
            if (htmlTags.Contains(tagName))
            {
                var newElement = new HtmlElement
                {
                    Name = tagName,
                    Parent = elementsStack.Peek()
                };
                // ניתוח Attributes
                var attributesRegex = new Regex("([^\\s]*?)=\"(.*?)\"");
                var matches = attributesRegex.Matches(trimmedLine);
                foreach (Match match in matches)
                {
                    var attributeName = match.Groups[1].Value.Trim();
                    var attributeValue = match.Groups[2].Value.Trim();

                    if (string.IsNullOrWhiteSpace(attributeValue))
                    {
                        continue;
                    }

                    if (attributeName == "class")
                    {
                        var classes = attributeValue.Split(' ')
                            .Where(c => !string.IsNullOrWhiteSpace(c))
                            .Distinct();

                        foreach (var className in classes)
                        {
                            if (!newElement.Classes.Contains(className))
                            {
                                newElement.Classes.Add(className);
                            }
                        }
                    }
                    else if (attributeName == "id")
                    {
                        if (string.IsNullOrWhiteSpace(newElement.Id))
                        {
                            newElement.Id = attributeValue;
                        }
                    }
                    else
                    {
                        var attributeEntry = $"{attributeName}={attributeValue}";
                        if (!newElement.Attributes.Contains(attributeEntry))
                        {
                            newElement.Attributes.Add(attributeEntry);
                        }
                    }
                }
                // הוספת האלמנט לרשימת הילדים של האלמנט הנוכחי
                elementsStack.Peek().Children.Add(newElement);

                // אם זו לא תגית void, הוספת האלמנט למחסנית
                if (!htmlVoidTags.Contains(tagName))
                {
                    elementsStack.Push(newElement);
                }
                else
                {
                    // לתגיות void אין תוכן פנימי
                    newElement.InnerHtml = string.Empty;
                    newElement.Children = null;
                }
            }
            else
            {
                // אם זו לא תגית, זהו תוכן פנימי של האלמנט הנוכחי
                elementsStack.Peek().InnerHtml += " " + trimmedLine;
            }
        }
        return root;
    }


    //SearchInHtml();


