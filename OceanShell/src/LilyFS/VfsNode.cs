namespace LilyFS
{
    public abstract class VfsNode
    {
        public string Name { get; set; } = "";
        public VfsDir? Parent { get; set; }
        public abstract bool IsDirectory { get; }
    }

    public sealed class VfsFile : VfsNode
    {
        public byte[] Content { get; set; } = [];
        public override bool IsDirectory => false;
    }

    public sealed class VfsDir : VfsNode
    {
        public List<VfsNode> Children { get; set; } = new();
        public override bool IsDirectory => true;

        public VfsNode? Find(string name) =>
            Children.Find(c => c.Name == name);
    }
}