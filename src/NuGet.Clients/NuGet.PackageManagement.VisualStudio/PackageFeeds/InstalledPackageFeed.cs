// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.VisualStudio.Internal.Contracts;

namespace NuGet.PackageManagement.VisualStudio
{
    /// <summary>
    /// Represents a package feed enumerating installed packages.
    /// </summary>
    public class InstalledPackageFeed : PlainPackageFeedBase
    {
        internal readonly IEnumerable<PackageCollectionItem> _installedPackages;
        internal readonly IPackageMetadataProvider _metadataProvider;

        public InstalledPackageFeed(
            IEnumerable<PackageCollectionItem> installedPackages,
            IPackageMetadataProvider metadataProvider)
        {
            if (installedPackages == null)
            {
                throw new ArgumentNullException(nameof(installedPackages));
            }
            _installedPackages = installedPackages;

            if (metadataProvider == null)
            {
                throw new ArgumentNullException(nameof(metadataProvider));
            }
            _metadataProvider = metadataProvider;
        }

        public override async Task<SearchResult<IPackageSearchMetadata>> ContinueSearchAsync(ContinuationToken continuationToken, CancellationToken cancellationToken)
        {
            var searchToken = continuationToken as FeedSearchContinuationToken;
            if (searchToken == null)
            {
                throw new InvalidOperationException(Strings.Exception_InvalidContinuationToken);
            }

            PackageIdentity[] packages = PerformLookup(_installedPackages.GetLatest(), searchToken);

            IEnumerable<IPackageSearchMetadata> items = await TaskCombinators.ThrottledAsync(
                packages,
                (p, t) => GetPackageMetadataAsync(p, searchToken.SearchFilter.IncludePrerelease, t),
                cancellationToken);

            return CreateResult(items);
        }

        internal static T[] PerformLookup<T>(IEnumerable<T> items, FeedSearchContinuationToken token) where T : PackageIdentity
        {
            return items
                .Where(p => p.Id.IndexOf(token.SearchString, StringComparison.OrdinalIgnoreCase) != -1)
                .OrderBy(p => p.Id)
                .Skip(token.StartIndex)
                .ToArray();
        }

        internal SearchResult<IPackageSearchMetadata> CreateResult(IEnumerable<IPackageSearchMetadata> items)
        {
            //  The packages were originally sorted which is important because we Skip based on that sort
            //  however the asynchronous execution has randomly reordered the set. So we need to resort. 
            SearchResult<IPackageSearchMetadata> result = SearchResult.FromItems(items.OrderBy(p => p.Identity.Id).ToArray());

            var loadingStatus = result.Any() ? LoadingStatus.NoMoreItems : LoadingStatus.NoItemsFound;
            result.SourceSearchStatus = new Dictionary<string, LoadingStatus>
            {
                { "Installed", loadingStatus }
            };

            return result;
        }

        internal async Task<IPackageSearchMetadata> GetPackageMetadataAsync(PackageIdentity identity, bool includePrerelease, CancellationToken cancellationToken)
        {
            // first we try and load the metadata from a local package
            var packageMetadata = await _metadataProvider.GetLocalPackageMetadataAsync(identity, includePrerelease, cancellationToken);
            if (packageMetadata == null)
            {
                // and failing that we go to the network
                packageMetadata = await _metadataProvider.GetPackageMetadataAsync(identity, includePrerelease, cancellationToken);
            }
            return packageMetadata;
        }
    }
}
