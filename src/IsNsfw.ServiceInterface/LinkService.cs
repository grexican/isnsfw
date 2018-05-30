using System;
using System.Collections.Generic;
using System.Text;
using IsNsfw.Model;
using IsNsfw.ServiceModel;
using ServiceStack;
using IsNsfw.Repository.Interface;
using IsNsfw.ServiceInterface.Validators;
using IsNsfw.ServiceModel.Types;

namespace IsNsfw.ServiceInterface
{
    public class LinkService : ServiceBase
        , IPost<CreateLinkRequest>
        , IGet<GetLinkRequest>
        , IPost<CreateLinkEventRequest>
    {
        const int StartKeyLength = 3;

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

            if(link.Key.IsNullOrEmpty())
            {
                var currentIterator = StartKeyLength * 10;
                do
                {
                    ++currentIterator;
                    link.Key = KeyValidator.GenerateKey(currentIterator / 10);
                } while (_linkRepo.KeyExists(link.Key));
            }

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

            return  _linkRepo.GetLinkResponse(link.Id);;
        }

        public object Post(CreateLinkEventRequest request)
        {
            var link = _linkRepo.GetByKey(request.Key);

            if(link == null) throw HttpError.NotFound($"Link with key '{request.Key}' not found.");

            UnitOfWork(() =>
            {
                var c = request.ConvertTo<LinkEvent>();
                c.LinkId = link.Id;
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

            return  _linkRepo.GetLinkResponse(link.Id);
        }

        public object Get(GetLinkRequest request)
        {
            var link = _linkRepo.GetByKey(request.Key);

            if(link == null) throw HttpError.NotFound($"Link with key '{request.Key}' not found.");

            return  _linkRepo.GetLinkResponse(link.Id);
        }
    }
}
