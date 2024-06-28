using StarKindred.Common.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Services;

namespace StarKindred.API.Endpoints.Towns;

[ApiController]
public sealed class MyRename
{
    [HttpPost("/towns/my/rename")]
    public async Task<ApiResponse> _(
        [FromBody] Request request,
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var town = await db.Towns.FirstAsync(t => t.UserId == session.UserId, cToken);

        town.Name = request.Name;

        await db.SaveChangesAsync(cToken);

        return new();
    }

    public sealed record Request(string Name)
    {
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                Transform(x => x.Name, n => n.Trim())
                    .NotEmpty().WithMessage("Must provide a name.")
                    .MaximumLength(20).WithMessage("Name cannot be longer than 20 characters.")
                ;
            }
        }
    }
}