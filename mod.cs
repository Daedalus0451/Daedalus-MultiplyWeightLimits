using MonoMod.Utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services.Mod;
using SPTarkov.Server.Core.Utils;
using System.Reflection;
using System.Runtime.CompilerServices;
using Path = System.IO.Path;

namespace Daedalus_MultiplyWeightLimits;

public record ModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; init; } = "se.daedalus.multiplyweightlimits";
    public override string Name { get; init; } = "MultiplyWeightLimits";
    public override string Author { get; init; } = "Daedalus";
    public override List<string>? Contributors { get; init; } = null;
    public override SemanticVersioning.Version Version { get; init; } = new("1.0.0");
    public override SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.1");

    public override List<string>? Incompatibilities { get; init; } = null;
    public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; } = null;
    public override string? Url { get; init; } = null;
    public override bool? IsBundleMod { get; init; } = false;
    public override string License { get; init; } = "MIT";
}

//Load after PreSptModLoader is complete with type priority +1
[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class MultiplyWeightLimits(
    ISptLogger<MultiplyWeightLimits> _logger, 
    ModHelper _modHelper, 
    DatabaseServer _databaseServer) : IOnLoad
{
    private Globals _globals;

    private readonly string pathToMod = _modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());

    public Task OnLoad()
    {
        _globals = _databaseServer.GetTables().Globals;

        //Get path to mod config
        var pathToConfig = Path.Combine(pathToMod, "Config");

        var modConfig = _modHelper.GetJsonDataFromFile<ModConfig>(pathToConfig, "config.json");

        _globals.Configuration.Stamina.BaseOverweightLimits.X *= modConfig.WeightLimitsMultiplier;
        _globals.Configuration.Stamina.BaseOverweightLimits.Y *= modConfig.WeightLimitsMultiplier;
        _globals.Configuration.Stamina.SprintOverweightLimits.X *= modConfig.WeightLimitsMultiplier;
        _globals.Configuration.Stamina.SprintOverweightLimits.Y *= modConfig.WeightLimitsMultiplier;
        _globals.Configuration.Stamina.WalkOverweightLimits.X *= modConfig.WeightLimitsMultiplier;
        _globals.Configuration.Stamina.WalkOverweightLimits.Y *= modConfig.WeightLimitsMultiplier;
        _globals.Configuration.Stamina.WalkSpeedOverweightLimits.X *= modConfig.WeightLimitsMultiplier;
        _globals.Configuration.Stamina.WalkSpeedOverweightLimits.Y *= modConfig.WeightLimitsMultiplier;

        return Task.CompletedTask;
    }
}

public class ModConfig
{
    public float WeightLimitsMultiplier { get; set; }
}
