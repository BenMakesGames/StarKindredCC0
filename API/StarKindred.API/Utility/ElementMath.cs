using StarKindred.Common.Entities;

namespace StarKindred.API.Utility;

public static class ElementMath
{
    private static readonly Dictionary<Element, List<Element>> ElementsDefeatedBy = new()
    {
        { Element.Earth, new List<Element>() { Element.Lightning } },
        { Element.Fire, new List<Element>() { Element.Plant, Element.Ice, Element.Metal } },
        { Element.Water, new List<Element>() { Element.Fire } },
        { Element.Plant, new List<Element>() { Element.Water, Element.Earth } },
        { Element.Ice, new List<Element>() { Element.Plant, Element.Water } },
        { Element.Lightning, new List<Element>() { Element.Plant, Element.Water, Element.Metal } },
        { Element.Metal, new List<Element>() { Element.Plant, Element.Earth } },
    };

    public static bool IsStrongAgainst(this Element e, Element other) =>
        ElementsDefeatedBy[e].Contains(other);

    public static bool IsWeakAgainst(this Element e, Element other) =>
        ElementsDefeatedBy[other].Contains(e);
    
    public static List<Element> GetStrongAgainst(this Element e) =>
        ElementsDefeatedBy[e];

    public static List<Element> GetWeakAgainst(this Element e) => ElementsDefeatedBy
        .Where(element => element.Value.Contains(e))
        .Select(element => element.Key)
        .ToList()
    ;
}