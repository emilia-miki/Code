namespace ShannonCoding;

public class Decoder<T> where T : notnull {
    private readonly TreeNode _tree;

    private class TreeNode
    {
        public TreeNode? LeftChild { get; set; }
        public TreeNode? RightChild { get; set; }
        public T? Data { get; set; }

        public bool IsLeaf()
        {
            return LeftChild == null && RightChild == null;
        }
    }

    public Decoder(Dictionary<T, Coding> codings)
    {
        _tree = new TreeNode();
        foreach(var pair in codings)
        {
            TraverseAndAddData(_tree, pair.Value.Bits, pair.Key);
        }
    }

    private void TraverseAndAddData(TreeNode root, IEnumerable<bool> coding, T data)
    {
        var node = root;
        foreach(var right in coding)
        {
            if (right)
            {
                node.RightChild ??= new TreeNode();
                node = node.RightChild;
            }
            else
            {
                node.LeftChild ??= new TreeNode();
                node = node.LeftChild;
            }
        }

        node.Data = data;
    }

    public List<T> Decode(IEnumerable<bool> data)
    {
        var node = _tree;
        var decoded = new List<T>();
        foreach (var right in data)
        {
            node = right ? node.RightChild : node.LeftChild;
            if (node!.IsLeaf())
            {
                decoded.Add(node.Data!);
                node = _tree;
            }
        }

        return decoded;
    }
}