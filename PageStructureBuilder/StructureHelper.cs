using System;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
using EPiServer.Security;
using EPiServer.ServiceLocation;

namespace PageStructureBuilder
{
    public class StructureHelper
    {
        public virtual TResult GetOrCreateChildPage<TResult>(
            PageReference parentLink, string pageName)
            where TResult : PageData
        {
            var child = GetExistingChild<TResult>(parentLink, pageName);
            if (child != null)
            {
                return child;
            }

            child = CreateChild<TResult>(parentLink, pageName);
            return child;
        }

        private TResult GetExistingChild<TResult>(
            PageReference parentLink, string pageName)
            where TResult : PageData
        {
            var children = DataFactory.Instance.GetChildren(parentLink);
            return children
                .OfType<TResult>()
                .FirstOrDefault(c => c.PageName.Equals(
                    pageName, StringComparison.InvariantCulture));
        }

        private TResult CreateChild<TResult>(
            PageReference parentLink, string pageName)
            where TResult : PageData
        {
            TResult child;
            var resultPageTypeId = ServiceLocator.Current.GetInstance<IContentTypeRepository>().Load<TResult>().ID;
            child = DataFactory.Instance.GetDefaultPageData(parentLink, resultPageTypeId) as TResult;
            child.PageName = pageName;
            DataFactory.Instance.Save(child, SaveAction.Publish, AccessLevel.NoAccess);
            return child;
        }
    }
}
