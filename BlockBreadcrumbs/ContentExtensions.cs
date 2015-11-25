using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using EPiServer.SpecializedProperties;

namespace BlockBreadcrumbs
{
    internal static class ContentExtensions
    {
        public static IContent AsIContent(this BlockData content)
        {
            return content as IContent;
        }

        public static IEnumerable<IContent> GetReferenceTree(this IContent content,
            IContentRepository repo, out int totalNumberOfReferences)
        {
            totalNumberOfReferences = 1;
            var referenceTree = new List<IContent>();
            var currentRef = content;
            while (currentRef != null && !(currentRef is PageData))
            {
                currentRef = GetFirstOwner(currentRef, repo, out totalNumberOfReferences);
                if (currentRef != null) referenceTree.Add(currentRef);
            }
            referenceTree.Reverse();
            return referenceTree;
        }

        private static IContent GetFirstOwner(IContent content, IContentRepository repo, out int totalNumberOfReferences)
        {
            var parentRef = repo.GetReferencesToContent(content.ContentLink, true).ToArray();
            totalNumberOfReferences = parentRef.Length;

            if (parentRef.Length == 1)
                return repo.Get<IContent>(parentRef[0].OwnerID);

            if (parentRef.Length > 1)
            {
                return null;
            }

            var parent = CheckUnpublished(content, repo);
            return parent;
        }

        private static IContent CheckUnpublished(IContent content, IContentRepository repo)
        {
            var allPending =
                repo.GetAll<IContent>(ContentReference.RootPage)
                .Select(p => p.LastVersion())
                .Where(version => !IsPublished(version))
                .Select(v => repo.Get<IContent>(v.ContentLink));

            foreach (var pendingPage in allPending)
            {
                foreach (var p in pendingPage.Property.OfType<PropertyContentArea>())
                {
                    var contentArea = p.Value as ContentArea;
                    if (contentArea != null
                        && contentArea.Items.Any(i => i.ContentLink.CompareToIgnoreWorkID(content.ContentLink)))
                        return pendingPage;
                }
            }
            return null;
        }

        private static ContentVersion LastVersion(this IContent content)
        {
            var versionRep = ServiceLocator.Current.GetInstance<IContentVersionRepository>();
            var lastVersion = versionRep.List(content.ContentLink).LastOrDefault();
            return lastVersion;
        }

        public static bool IsPublished(this IContent content)
        {
            var lastVersion = content.LastVersion();
            return IsPublished(lastVersion);
        }

        private static bool IsPublished(ContentVersion version)
        {
            return version != null && version.Status == VersionStatus.Published;
        }

        public static IEnumerable<T> GetAll<T>(this IContentRepository contentRepository, ContentReference fromContentReference) where T : IContent
        {
            return contentRepository.GetItems(contentRepository.GetDescendents(fromContentReference)).OfType<T>();
        }

        public static IEnumerable<ContentReference> GetDescendents(this IContentRepository contentRepository, ContentReference contentLink)
        {
            return contentRepository.GetDescendents(contentLink);
        }

        public static IEnumerable<IContent> GetItems(this IContentRepository contentRepository, IEnumerable<ContentReference> contentReferences)
        {
            return contentRepository.GetItems(contentReferences, LanguageSelector.AutoDetect());
        }
    }
}
