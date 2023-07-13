using HtmlAgilityPack;

namespace CardboardBox.Http;

/// <summary>
/// Provides an easy way of traversing HTML sequential nodes
/// </summary>
public class HtmlTraverser
{
    private readonly HtmlNode? _targetNode;

    /// <summary>
    /// The current index of the traverser
    /// </summary>
    public int Index { get; set; } = 0;

    /// <summary>
    /// The <see cref="HtmlNode"/> at the current <see cref="Index"/>
    /// </summary>
    public HtmlNode? Current => _targetNode?.ChildNodes[Index];

    /// <summary>
    /// Whether or not the traverser is in a valid state
    /// </summary>
    public bool Valid => _targetNode != null && Index < _targetNode.ChildNodes.Count;

    /// <summary>
    /// Creates an <see cref="HtmlTraverser"/> targetting the child nodes of the given <paramref name="xpath"/>
    /// </summary>
    /// <param name="rootDoc">The HTML document</param>
    /// <param name="xpath">The XPath to the parent whose children is to be traversed</param>
    public HtmlTraverser(HtmlDocument rootDoc, string xpath)
    {
        _targetNode = rootDoc.DocumentNode.SelectSingleNode(xpath);
    }

    /// <summary>
    /// Creates an <see cref="HtmlTraverser"/> for the children fo the given node
    /// </summary>
    /// <param name="targetNode"></param>
    public HtmlTraverser(HtmlNode? targetNode)
    {
        _targetNode = targetNode;
    }

    /// <summary>
    /// Returns all of the children from the current point until the predicate is met or the end of the children is reached
    /// </summary>
    /// <param name="predicate">The predicate that indicates what child to stop at</param>
    /// <returns>All of the childent until the predicate is met</returns>
    public IEnumerable<HtmlNode> EverythingUntil(Func<HtmlNode, bool> predicate)
    {
        if (_targetNode == null) yield break;

        while (Index < _targetNode.ChildNodes.Count)
        {
            var node = Current;
            Index++;
            if (node == null) continue;
            if (predicate(node)) yield break;
            yield return node;
        }
    }

    /// <summary>
    /// Returns everything until the given node name is reached or the end of the children is reached
    /// </summary>
    /// <param name="name">The name of the node to traverse until</param>
    /// <returns>All of the children until the given node is met</returns>
    public IEnumerable<HtmlNode> EverythingUntil(string name) => EverythingUntil(node => node.Name == name);

    /// <summary>
    /// Moves the traverser until the predicate is met or the end of the children is reached and returns the element at that position
    /// </summary>
    /// <param name="predicate">The predicates that indicates the child to return</param>
    /// <returns>The first child that met the predicate or null if no children were found</returns>
    public HtmlNode? MoveUntil(Func<HtmlNode, bool> predicate)
    {
        if (_targetNode == null) return null;

        while (Index < _targetNode.ChildNodes.Count)
        {
            var node = Current;
            Index++;
            if (node == null) break;
            if (predicate(node)) return node;
        }

        return null;
    }

    /// <summary>
    /// Moves the traverser until the given node name is reached or the end of the children is reached and returns the element at that position
    /// </summary>
    /// <param name="name">The name of the child to stop at</param>
    /// <returns>The first child with the given name or null if no children were found</returns>
    public HtmlNode? MoveUntil(string name) => MoveUntil(node => node.Name == name);

    /// <summary>
    /// Moves the traverser until the <paramref name="after"/> predicate is met and then returns all of the children until the <paramref name="until"/> predicate is met
    /// </summary>
    /// <param name="after">The predicate that indicates the start point of the children to return</param>
    /// <param name="until">The predicate that indicates the child to stop returning elements at</param>
    /// <returns>All of the children that <paramref name="after"/> predicate is met but before the <paramref name="until"/> predicate is met</returns>
    public IEnumerable<HtmlNode> AfterUntil(Func<HtmlNode, bool> after, Func<HtmlNode, bool> until)
    {
        var af = MoveUntil(after);
        if (af == null) yield break;

        foreach (var node in EverythingUntil(until))
            yield return node;
    }

    /// <summary>
    /// Moves back up the children until it finds a node that meets the predicate
    /// </summary>
    /// <param name="predicate">The predicate that indicates the node to stop at</param>
    /// <returns>The node that met the predicate first</returns>
    public HtmlNode? BackUntil(Func<HtmlNode, bool> predicate)
    {
        if (_targetNode == null) return null;

        while (Index > 0)
        {
            var node = Current;
            Index--;
            if (node == null) break;
            if (predicate(node)) return node;
        }

        return null;
    }

    /// <summary>
    /// Moves the traverser until if finds an element that meets one of the given predicates and then returns the element and the index of the predicate that was met
    /// </summary>
    /// <param name="preds">The predicates to match against</param>
    /// <returns>The <see cref="HtmlNode"/> that matched the predicate and the index of the predicate that it met. (null and -1 respectively if it couldn't find any matches)</returns>
    public (HtmlNode? node, int index) UntilOneOf(params Func<HtmlNode, bool>[] preds)
    {
        if (_targetNode == null) return (null, -1);

        while (Index < _targetNode.ChildNodes.Count)
        {
            var node = Current;
            Index++;
            if (node == null) continue;
            var index = Array.FindIndex(preds, x => x(node));
            if (index != -1) return (node, index);
        }

        return (null, -1);
    }
}
