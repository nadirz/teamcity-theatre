using System;
using System.Linq;
using System.Threading.Tasks;
using TeamCityTheatre.Core.ApplicationModels;
using TeamCityTheatre.Core.DataServices;
using TeamCityTheatre.Core.Models;
using TeamCityTheatre.Core.QueryServices.Models;

namespace TeamCityTheatre.Core.QueryServices
{
    public class TileService : ITileService
    {
        readonly IBuildDataService _buildDataService;

        public TileService(IBuildDataService buildDataService)
        {
            _buildDataService = buildDataService ?? throw new ArgumentNullException(nameof(buildDataService));
        }

        public async Task<TileData> GetLatestTileDataAsync(View view, Tile tile)
        {
            try
            {
                var rawBuilds = await _buildDataService.GetBuildsOfBuildConfigurationAsync(tile.BuildConfigurationId, 20);
                var buildsOrderByStartDate = rawBuilds.OrderByDescending(b => b.StartDate).ToList();
                var defaultBranchBuild = buildsOrderByStartDate.FirstOrDefault(b => b.IsDefaultBranch);
                var nonDefaultBranchBuilds = buildsOrderByStartDate.Where(b => !b.IsDefaultBranch)
                  .GroupBy(b => b.BranchName)
                  .Select(buildsPerBranch => buildsPerBranch.OrderByDescending(b => b.StartDate).First())
                  .Take(defaultBranchBuild != null
                    ? view.DefaultNumberOfBranchesPerTile - 1
                    : view.DefaultNumberOfBranchesPerTile);
                var builds = defaultBranchBuild != null
                  ? new[] { defaultBranchBuild }.Concat(nonDefaultBranchBuilds)
                  : nonDefaultBranchBuilds;
                return new TileData
                {
                    Id = tile.Id,
                    Label = tile.Label,
                    Builds = builds.ToList()
                };
            }
            catch
            {
                return new TileData
                {
                    Id = tile.Id,
                    Label = tile.Label,
                    Builds = new[] {new Build() { BranchName = "Build ID changed!" } }
                };
            }
        }
    }
}