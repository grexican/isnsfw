using IsNsfw.ServiceModel.Types;

namespace IsNsfw.Mvc.Models
{
    public class TagViewModel : TagResponse
    {
        public bool IsSelected { get; set ;}
    }
}