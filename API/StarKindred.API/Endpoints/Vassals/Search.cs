using StarKindred.API.Utility;
using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Extensions;
using StarKindred.API.Services;

namespace StarKindred.API.Endpoints.Vassals;

[ApiController]
public sealed class Search
{
    [HttpGet("/vassals/search")]
    public async Task<ApiResponse<PaginatedResults<VassalDto>>> Handle(
        [FromQuery] Request request,
        [FromServices] Db db, [FromServices] ICurrentUser currentUser,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);
        
        var results = await db.Vassals
            .Where(v => v.UserId == session.UserId)
            .WithName(request.Name)
            .WithMission(request.OnMission)
            .WithLeader(request.IsLeader)
            .WithElement(request.Element)
            .WithNature(request.Nature)
            .WithAstrologicalSign(request.Sign)
            .WithTag(request.Tag)
            .OrderByDescending(v => v.Level)
                .ThenBy(v => v.Name)
            .Select(v => new VassalDto(
                v.Id,
                v.Name,
                v.Favorite,
                v.Species,
                v.Portrait,
                v.Level,
                v.Willpower,
                v.Element,
                v.Sign,
                v.Nature,
                v.CreatedOn,
                v.Weapon == null ? null : new(v.Weapon!.Image, v.Weapon.Level, v.Weapon.PrimaryBonus, v.Weapon.Level >= 3 ? v.Weapon.SecondaryBonus : null),
                v.StatusEffects!.Select(s => s.Type).ToList(),
                v.Tags!.Select(t => new TagDto(t.Title, t.Color)).ToList(),
                v.IsOnAMission,
                v.Leader != null
            ))
            .AsSplitQuery()
            .AsPaginatedResultsAsync(request.Page, request.PageSize, cToken)
        ;

        return new(results);
    }

    public sealed record Request(string? Name, bool? OnMission, bool? IsLeader, Element? Element, AstrologicalSign? Sign, Nature? Nature, string? Tag, int PageSize = 60, int Page = 1)
    {
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.PageSize).GreaterThanOrEqualTo(5).LessThanOrEqualTo(60).WithMessage("Page size must be between 5 and 60.");
                RuleFor(x => x.Page).PageNumber();
            }
        }
    }

    public sealed record VassalDto(
        Guid Id,
        string Name,
        bool Favorite,
        Species Species,
        string Portrait,
        int Level,
        int Willpower,
        Element Element,
        AstrologicalSign Sign,
        Nature Nature,
        DateTimeOffset RecruitDate,
        WeaponDto? Weapon,
        List<StatusEffectType> StatusEffects,
        List<TagDto> Tags,
        bool OnMission,
        bool Leader
    );
    public sealed record WeaponDto(string Image, int Level, WeaponBonus PrimaryBonus, WeaponBonus? SecondaryBonus);
    public sealed record TagDto(string Title, string Color);
}

public static class IQueryableExtensions
{
    public static IQueryable<Vassal> WithName(this IQueryable<Vassal> query, string? searchText) =>
        string.IsNullOrWhiteSpace(searchText)
            ? query
            : query.Where(v => v.Name.ToLower().Contains(searchText.Trim().ToLower()));

    public static IQueryable<Vassal> WithMission(this IQueryable<Vassal> query, bool? onMission) => onMission switch
    {
        true => query.Where(v => v.MissionId != null || v.TimedMissionId != null || v.UserAdventureStepInProgressId != null),
        false => query.Where(v => v.MissionId == null && v.TimedMissionId == null && v.UserAdventureStepInProgressId == null),
        _ => query
    };

    public static IQueryable<Vassal> WithLeader(this IQueryable<Vassal> query, bool? isLeader) => isLeader switch
    {
        true => query.Where(v => v.Leader != null),
        false => query.Where(v => v.Leader == null),
        _ => query
    };

    public static IQueryable<Vassal> WithElement(this IQueryable<Vassal> query, Element? element) =>
        element == null
            ? query
            : query.Where(v => v.Element == element);
    
    public static IQueryable<Vassal> WithNature(this IQueryable<Vassal> query, Nature? nature) =>
        nature == null
            ? query
            : query.Where(v => v.Nature == nature);
    
    public static IQueryable<Vassal> WithAstrologicalSign(this IQueryable<Vassal> query, AstrologicalSign? sign) =>
        sign == null
            ? query
            : query.Where(v => v.Sign == sign);
    
    public static IQueryable<Vassal> WithTag(this IQueryable<Vassal> query, string? tag) =>
        string.IsNullOrWhiteSpace(tag)
            ? query
            : query.Where(v => v.Tags!.Any(t => EF.Functions.Like(t.Title, tag.Trim())));
}