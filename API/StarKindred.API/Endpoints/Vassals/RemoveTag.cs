using StarKindred.Common.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;

namespace StarKindred.API.Endpoints.Vassals;

[ApiController]
public sealed class RemoveTag
{
    [HttpPost("/vassals/{vassalId:guid}/tags/delete")]
    public async Task<ApiResponse> _(
        Guid vassalId,
        [FromBody] RemoveTagRequest request,
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var vassal = await db.Vassals
            .Where(v => v.Id == vassalId && v.UserId == session.UserId)
            .Include(v => v.Tags)
            .AsSplitQuery() // TODO: not profiled
            .FirstOrDefaultAsync(cToken)
            ?? throw new NotFoundException("Vassal not found.");

        var tagTitle = request.Title.Trim();

        var tag = vassal.Tags!.FirstOrDefault(t => t.Title == tagTitle)
            ?? throw new UnprocessableEntity($"{vassal.Name} doesn't have that tag.");

        // if this is the only vassal with the tag, remove the tag entirely
        var tagUsageCount = await db.UserVassalTags
            .Where(t => t.Id == tag.Id)
            .Select(t => t.Vassals!.Count)
            .FirstOrDefaultAsync(cToken);

        if (tagUsageCount == 1)
            db.UserVassalTags.Remove(tag);
        else
            vassal.Tags!.Remove(tag);
        
        await db.SaveChangesAsync(cToken);

        return new();
    }

    public sealed record RemoveTagRequest(string Title)
    {
        public sealed class Validator : AbstractValidator<RemoveTagRequest>
        {
            public Validator()
            {
                Transform(x => x.Title, title => title?.Trim() ?? "").NotEmpty().WithMessage("Title is required.");
            }
        }
    }
}