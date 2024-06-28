using Microsoft.AspNetCore.Mvc;
using StarKindred.API.Entities;
using StarKindred.API.Services;
using StarKindred.API.Utility.Buildings;
using StarKindred.API.Utility.Buildings.Powers;
using StarKindred.Common.Services;

namespace StarKindred.API.Endpoints.Buildings;

[ApiController]
public sealed class ActivateBuildingPower
{
    [HttpPost("/buildings/{buildingId:guid}/activatePower")]
    public async Task<ApiResponse<BuildingPowerResponse>> _(
        Guid buildingId,
        BuildingPowerRequest request,
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        [FromServices] Random rng,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var (building, technologies) = await BuildingPowerHelpers.PrepareBuildingPower(db, session.UserId, buildingId, request.Choice, cToken);

        var (message, resourcesGained, unlockedDecorations) = request.Choice switch
        {
            Level20Power.Fishery_BoatRide => await FisheryPowers.DoBoatRide(db, rng, session, cToken),
            Level20Power.Fishery_Market => await FisheryPowers.DoFishMarket(db, session, technologies, cToken),
            Level20Power.GoldMine_Gems => await GoldMinePowers.DoGemPower(db, session, rng, technologies, cToken),
            Level20Power.Hunter_Hunt => await HunterPowers.DoMonsterHunter(db, session, rng, cToken),
            Level20Power.IronMine_Repair => await IronMinePowers.DoRepairPower(db, session, cToken),
            Level20Power.IronMine_Sword => IronMinePowers.DoGetSword(db, rng, session, technologies),
            Level20Power.Lumberyard_MoreWood => await LumberyardPowers.DoMoreWood(db, session, cToken),
            Level20Power.Lumberyard_Axe => LumberyardPowers.DoGetAxe(db, rng, session, technologies),
            Level20Power.Lumberyard_FinishedGoods => await LumberyardPowers.DoFinishedGoods(db, session, technologies, cToken),
            Level20Power.MarbleQuarry_Veins => await MarbleQuarryPowers.DoMetallicVein(db, session, rng, technologies, cToken),
            Level20Power.Palace_Knight => await PalacePowers.DoKnighting(db, session, rng, cToken),
            Level20Power.Palace_AttractSettlers => await PalacePowers.DoAttractSettlers(db, session, rng, cToken),
            Level20Power.TradeDepot_Iron or
                Level20Power.TradeDepot_Marble or
                Level20Power.TradeDepot_Meat or
                Level20Power.TradeDepot_Stone or
                Level20Power.TradeDepot_Wheat or
                Level20Power.TradeDepot_Wine or
                Level20Power.TradeDepot_Wood => await TradeDepotPowers.DoTrade(db, session, request.Choice, cToken),
            Level20Power.Pasture_Homestead => await PasturePowers.DoHomesteading(db, session, rng, cToken),
            Level20Power.Pasture_Scythe => PasturePowers.DoGetScythe(db, rng, session, technologies),
            Level20Power.Temple_Miracle => await TemplePowers.DoMiracle(db, session, rng, cToken),
            Level20Power.Temple_Wand => TemplePowers.DoGetWand(db, rng, session, technologies),
            Level20Power.Vineyard_Party => await VineyardPowers.DoEntertain(db, session, cToken),
            Level20Power.Vineyard_Sell => await VineyardPowers.DoSell(db, session, cToken),
            Level20Power.Pasture_Vineyard_Sacrifice => await PasturePowers.DoSacrifice(db, session, technologies, cToken),
            _ => throw new Exception("This building power has not been implemented! :( Ben has been notified, and will hopefully fix this soon!")
        };

        building.PowerLastActivatedOn = DateTimeOffset.UtcNow.Date;

        await db.SaveChangesAsync(cToken);

        return new(new(building.PowerLastActivatedOn.AddDays(1), resourcesGained, unlockedDecorations))
        {
            Messages = new()
            {
                ApiMessage.Info(message)
            }
        };
    }

    public sealed record BuildingPowerRequest(Level20Power Choice);
    public sealed record BuildingPowerResponse(DateTimeOffset PowersAvailableOn, ResourceQuantity? Resources, bool UnlockedDecorations);
}