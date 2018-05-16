namespace IsNsfw.Model
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
    }
}