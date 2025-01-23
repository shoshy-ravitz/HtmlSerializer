using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HtmlSerializer
{
    public class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Attributes { get; set; }
        public List<string> Classes { get; set; }
        public String InnerHtml { get; set; }
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; }
        public HtmlElement()
        {
            Id = "";
            Name = "";
            InnerHtml = "";
            Attributes = new List<string>();
            Classes = new List<string>();
            Children = new List<HtmlElement>();
            Parent = null;
        }
        public override string ToString()
        {
            string res = $"<{Name}";
            if (Id != "")
            {
                res+=$"id={Id} ";
            }
            if (Classes.Count > 0)
            {
                res+=$" class={String.Join(" ", Classes)} ";
            }
            res+=$"> {InnerHtml} </{Name}>";
            return res ;
        }
        public IEnumerable<HtmlElement> Descendants()
        {
            Queue<HtmlElement> queue = new Queue<HtmlElement>();
            queue.Enqueue(this);

            while (queue.Count > 0)
            {
                HtmlElement current = queue.Dequeue();
                yield return current;

                if (current.Children != null)
                {
                    foreach (var child in current.Children)
                    {
                        queue.Enqueue(child);
                    }
                }
                
            }
        }
        public IEnumerable<HtmlElement> Ancestors()
        {
            HtmlElement current = this;
            while (current.Parent != null)
            {
                yield return Parent;
                current = current.Parent;
            }
        }
    }
    public static class HtmlElementExtensions
    {
        public static HashSet<HtmlElement> FindBySelector(this HtmlElement element, Selector selector)
        {
            var result = new HashSet<HtmlElement>();
            FindBySelectorRecursive(element, selector, result);
            return result;
        }

        private static void FindBySelectorRecursive(HtmlElement element, Selector selector, HashSet<HtmlElement> result)
        {
            if (selector == null) return;

            var matchingElements = element.Descendants().Where(el =>
                (string.IsNullOrEmpty(selector.TagName) || el.Name == selector.TagName) &&
                (string.IsNullOrEmpty(selector.Id) || el.Id == selector.Id) &&
                (!selector.Classes.Any() || selector.Classes.All(cls => el.Classes.Contains(cls)))
            ).ToList();

            if (selector.Child == null)
            {
                matchingElements.ForEach(r => result.Add(r));
            }
            else
            {
                foreach (var match in matchingElements)
                {
                    FindBySelectorRecursive(match, selector.Child, result);
                }
            }
        }
    }
}




    // public IEnumerable<HtmlElement> FindSelectorInHtml(Selector selector,HtmlElement htmlElement ,List<HtmlElement> res)
    //{
    //    if(selector==null)
    //    {
    //        return;
    //    }
    //    bool isTagName=false;
    //    bool isId = false;
    //    bool isClasses = true;

    //    var descendant =  htmlElement.Descendants().ToList();
    //    foreach(var d in descendant)
    //    {
    //        FindSelectorInHtml(selector,htmlElement,res);
    //    }
    //    if (selector.TagName == descendant.First().Name || selector.TagName == "")
    //    {
    //            isTagName = true;
    //    }
    //    if (selector.Id == descendant.First().Id || selector.Id == "")
    //    {
    //         isId = true;
    //    }
    //    foreach (var c in selector.Classes)
    //    {
    //         if (!descendant.First().Classes.Contains(c))
    //         {
    //               isClasses = false;
    //         }
    //    }
    //    if(isTagName && isId &isClasses)//all good
    //    {
    //         foreach (var child in descendant.First().Children)
    //         {
    //                 FindSelectorInHtml(selector.Child, child, res);
    //         }
    //    }
    //    else
    //    {
    //        foreach (var child in descendant)
    //        {
    //            FindSelectorInHtml(selector, child, res);
    //        }
    //    }
    //}

//div class
//< div >
//    < i >
//        < p class=”class-name”></ p >
//    </ i >
//    < i >
//        < p class=”class-name”></ p >
//    </ i >
//</ div >
