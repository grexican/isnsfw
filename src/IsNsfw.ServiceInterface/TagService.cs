using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IsNsfw.Repository.Interface;
using IsNsfw.ServiceModel;
using IsNsfw.ServiceModel.Types;
using ServiceStack;

namespace IsNsfw.ServiceInterface
{
    public class TagService : ServiceBase
        , IGet<GetTagsRequest>
    {
        private readonly ITagRepository _tagRepo;

        public TagService(ITagRepository tagRepo)
        {
            _tagRepo = tagRepo;
        }

        public object Get(GetTagsRequest request)
        {
            return new TagsResponse(_tagRepo.GetOrderedTags());
        }
    }
}
