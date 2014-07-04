// Guids.cs
// MUST match guids.h
using System;

namespace Buaa.FLToolPackage
{
    static class GuidList
    {
        public const string guidFLToolPackagePkgString = "2fd83326-2379-4eff-847d-1ab466d7f5c1";
        public const string guidFLToolPackageCmdSetString = "0bb4dc5a-58aa-49e6-a207-db3968ae607e";
        public const string guidToolWindowPersistanceString = "162ae67d-0dc0-4c67-993d-e1a279af421c";

        public static readonly Guid guidFLToolPackageCmdSet = new Guid(guidFLToolPackageCmdSetString);
    };
}