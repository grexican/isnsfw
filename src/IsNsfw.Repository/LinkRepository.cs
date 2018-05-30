using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IsNsfw.Model;
using IsNsfw.Repository.Interface;
using IsNsfw.ServiceModel.Types;
using ServiceStack;
using ServiceStack.Data;
using ServiceStack.OrmLite;

namespace IsNsfw.Repository
{
    public class LinkRepository : IntRepositoryBase<Link>, ILinkRepository
    {
        public LinkRepository(IDbConnectionFactory factory) : base(factory)
        {
        }

        public bool KeyExists(string key)
        {
            return Execute(db => db.Exists<Link>(m => m.Key == key));
        }

        public Link GetByKey(string key)
        {
            return Execute(db => db.Single<Link>(m => m.Key == key));
        }

        public void SetLinkTags(int linkId, IEnumerable<LinkTag> linkTags)
        {
            Execute(db =>
            {
                db.Delete<LinkTag>(m => m.LinkId == linkId);
                db.InsertAll(linkTags.Where(m => m.LinkId == linkId));
            });
        }

        public LinkResponse GetLinkResponse(int linkId)
        {
            return Execute(db =>
            {
                var query = db.From<Link>()
                                .LeftJoin<Link, LinkTag>( (l, lt) => l.Id == lt.LinkId)
                                .LeftJoin<LinkTag, Tag>( (lt, t) => lt.TagId == t.Id)
                                .Where(l => l.Id == linkId);

                var results = db.SelectMulti<Link, Tag>(query);

                var link = results[0].Item1.ConvertTo<LinkResponse>();
                link.Tags = results.Select(m => m.Item2.Key).ToHashSet();

                return link;
            });
        }

        public void CreateLinkEvent(LinkEvent linkEvent)
        {
            Execute(db => db.Save(linkEvent));
        }

        public void IncrementTotalViews(int linkId)
        {
            Execute(db => db.UpdateAdd(() => new Link { TotalViews = 1 }, where: m => m.Id == linkId));
        }

        public void IncrementClickThroughs(int linkId)
        {
            Execute(db => db.UpdateAdd(() => new Link { TotalClickThroughs = 1 }, where: m => m.Id == linkId));
        }

        public void IncrementPreviews(int linkId)
        {
            Execute(db => db.UpdateAdd(() => new Link { TotalPreviews = 1 }, where: m => m.Id == linkId));
        }

        public void IncrementTurnBacks(int linkId)
        {
            Execute(db => db.UpdateAdd(() => new Link { TotalTurnBacks = 1 }, where: m => m.Id == linkId));
        }
    }
}
