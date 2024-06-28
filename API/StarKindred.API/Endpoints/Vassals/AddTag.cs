using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility;

namespace StarKindred.API.Endpoints.Vassals;

[ApiController]
public sealed class AddTag
{
    [HttpPost("/vassals/{vassalId:guid}/tags")]
    public async Task<ApiResponse> _(
        Guid vassalId,
        [FromBody] AddTagRequest request,
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

        if(vassal.Tags!.Count >= 5)
            throw new UnprocessableEntity("A Vassal can only have up to 5 tags.");
        
        var tagTitle = request.Title.Trim();
        
        if(vassal.Tags!.Any(t => t.Title == tagTitle))
            throw new UnprocessableEntity($"{vassal.Name} already has that tag.");

        var existingTag = await db.UserVassalTags
            .FirstOrDefaultAsync(t => t.UserId == session.UserId && t.Title == tagTitle, cToken);

        if (existingTag == null)
        {
            var existingTagCount = await db.UserVassalTags.CountAsync(t => t.UserId == session.UserId, cToken);
            
            if(existingTagCount == 20)
                throw new UnprocessableEntity("You can't have more than 20 tags.");
            
            var tagColor = request.Color?.Trim() ?? "";
            
            if(!ColorHelpers.IsValid(tagColor))
                throw new UnprocessableEntity("Must specify a color.");
            
            var newTag = new UserVassalTag()
            {
                Title = tagTitle,
                Color = tagColor,
                UserId = session.UserId,
                Vassals = new List<Vassal>() { vassal },
            };
            
            db.UserVassalTags.Add(newTag);
        }
        else
            vassal.Tags!.Add(existingTag);
        
        await db.SaveChangesAsync(cToken);

        return new();
    }

    public sealed record AddTagRequest(string Title, string? Color)
    {
        public sealed class Validator : AbstractValidator<AddTagRequest>
        {
            public Validator()
            {
                Transform(x => x.Title, title => title.Trim())
                    .NotEmpty().WithMessage("Tag name is required.")
                    .MaximumLength(20).WithMessage("Tag name may not be longer than 20 characters.");
            }
        }
    }
}