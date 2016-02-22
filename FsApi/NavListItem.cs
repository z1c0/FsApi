namespace FsApi
{
    public class NavListItem :KeyLabelItem
    {
        public string name { get; internal set; }
        public byte ItemType { get; internal set; }
        public byte subtype { get; internal set; }
    }
}
