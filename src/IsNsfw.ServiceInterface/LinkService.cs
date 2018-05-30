using System;
using System.Collections.Generic;
using System.Text;
using IsNsfw.Model;
using IsNsfw.ServiceModel;
using ServiceStack;
using IsNsfw.Repository.Interface;
using IsNsfw.ServiceModel.Types;

namespace IsNsfw.ServiceInterface
{
    public class LinkService : ServiceBase
        , IPost<CreateLinkRequest>
        , IPostVoid<CreateLinkEventRequest>
    {
        private readonly ILinkRepository _linkRepo;
        private readonly ITagRepository _tagRepo;

        public LinkService(ILinkRepository linkRepo
                    , ITagRepository tagRepo)
        {
            _linkRepo = linkRepo;
            _tagRepo = tagRepo;
        }

        public object Post(CreateLinkRequest request)
        {
            var link = request.ConvertTo<Link>();
            link.SessionId = Session.Id;
            link.CreatedAt = DateTime.UtcNow;

            UnitOfWork(() =>
            {
                _linkRepo.Create(link);

                if(request.Tags != null)
                {
                    link.LinkTags = new List<LinkTag>();

                    var tagsDict = _tagRepo.GetTagsDictionary();

                    foreach(var tag in request.Tags)
                    {
                        if(!tagsDict.ContainsKey(tag))
                            throw HttpError.NotFound($"Tag '{tag}' not found.");

                        var linkTag = new LinkTag()
                        {
                            LinkId = link.Id,
                            TagId  = tagsDict[tag].Id
                        };

                        link.LinkTags.Add(linkTag);
                    }

                    _linkRepo.SetLinkTags(link.Id, link.LinkTags);
                }
            });

            var ret = _linkRepo.GetLinkResponse(link.Id);

            return ret;
        }

        public void Post(CreateLinkEventRequest request)
        {
            UnitOfWork(() =>
            {
                var c = request.ConvertTo<LinkEvent>();
                c.LinkId = request.Id;
                c.CreatedAt = DateTime.UtcNow;
                c.SessionId = Session.Id;
                _linkRepo.CreateLinkEvent(c);

                switch(c.LinkEventType)
                {
                    case LinkEventType.View:
                        _linkRepo.IncrementTotalViews(c.LinkId);
                        break;

                    case LinkEventType.ClickThrough:
                        _linkRepo.IncrementClickThroughs(c.LinkId);
                        break;

                    case LinkEventType.Preview:
                        _linkRepo.IncrementPreviews(c.LinkId);
                        break;

                    case LinkEventType.TurnBack:
                        _linkRepo.IncrementTurnBacks(c.LinkId);
                        break;

                    default:
                        throw new System.ArgumentException($"Unknown {nameof(LinkEventType)} value '{c.LinkEventType}'", nameof(c.LinkEventType));
                }
            });
        }
    }
}
