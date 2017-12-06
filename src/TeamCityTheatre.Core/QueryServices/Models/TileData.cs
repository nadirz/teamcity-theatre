using System;
using System.Collections.Generic;
using System.Linq;
using TeamCityTheatre.Core.Models;

namespace TeamCityTheatre.Core.QueryServices.Models {
  public class TileData {
    public Guid Id { get; set; }
    public string Label { get; set; }

    public IList<IDetailedBuild> Builds { get; set; }

    public BuildStatus CombinedBuildStatus
    {
      get
      {
        if (Builds == null) { return BuildStatus.Unknown; }

        var defaultBranchBuild = Builds.FirstOrDefault(b => b.IsDefaultBranch);
        return defaultBranchBuild?.Status ?? (Builds.All(b => b.Status == BuildStatus.Success) ? BuildStatus.Success : BuildStatus.Error);
      }
    }
  }
}