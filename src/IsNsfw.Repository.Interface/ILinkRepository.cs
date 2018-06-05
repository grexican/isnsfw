using System;
using System.Collections.Generic;
using System.Text;
using IsNsfw.Model;
using IsNsfw.ServiceModel.Types;

namespace IsNsfw.Repository.Interface
{
    public interface ILinkRepository : IIntRepository<Link>
    {
        bool KeyExists(string key);
        Link GetByKey(string key);
        void SetLinkTags(int linkId, IEnumerable<LinkTag> linkTags);
        LinkResponse GetLinkResponse(int linkId);
        void CreateLinkEvent(LinkEvent linkEvent);
        void IncrementTotalViews(int linkId);
        void IncrementClickThroughs(int linkId);
        void IncrementPreviews(int linkId);
        void IncrementTurnBacks(int linkId);
        void SetScreenshotUrl(int linkId, string screenshotUrl);
    }
}
