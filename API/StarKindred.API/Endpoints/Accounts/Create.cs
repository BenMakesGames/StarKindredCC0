using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;

namespace StarKindred.API.Endpoints.Accounts;

[ApiController]
public sealed class Create
{
    public static readonly string[] AvailablePortraits =
    {
        "priestess", "sailor", "sword"
    };
    
    [HttpPost("/accounts/create")]
    public async Task<ApiResponse> _(
        Request request,
        [FromServices] Db db,
        [FromServices] IPassphraseHasher passphraseHasher,
        CancellationToken cToken
    )
    {
        await new Request.Validator().ValidateAndThrowAsync(request, cToken);
        
        if(await db.Users.AnyAsync(u => u.Email == request.Email.Trim(), cToken))
        {
            throw new UnprocessableEntity("Email address is already in use.");
        }

        var user = new User()
        {
            Name = request.PersonalName.Trim(),
            Email = request.Email.Trim(),
            Passphrase = passphraseHasher.Hash(request.Passphrase),
        };

        var town = new Town()
        {
            User = user,
            NextRumor = DateTimeOffset.UtcNow.AddHours(24 + 8).Date,
        };

        var sign = ((int)((DateTimeOffset.UtcNow.DayOfYear - 1) / (365 / 13f))) % 13;
        var element = DateTimeOffset.UtcNow.DayOfYear % Enum.GetValues<Element>().Length;

        var vassal = new Vassal()
        {
            User = user,
            Name = request.VassalName.Trim(),
            Portrait = request.Portrait,
            Level = 0,
            Willpower = 1,
            Element = (Element)element,
            Species = Species.Human,
            Sign = (AstrologicalSign)sign,
            Nature = Nature.Evangelist,
        };

        var building = new Building()
        {
            User = user,
            Position = 0,
            Type = BuildingType.Palace,
            LastHarvestedOn = DateTimeOffset.UtcNow.Subtract(TimeSpan.FromHours(6)),
        };

        var resources = new List<Resource>()
        {
            new() { User = user, Type = ResourceType.Wood, Quantity = 80 },
            new() { User = user, Type = ResourceType.Wheat, Quantity = 50 },
            new() { User = user, Type = ResourceType.Gold, Quantity = 20 },
        };

        db.Users.Add(user);
        db.Towns.Add(town);
        db.Vassals.Add(vassal);
        db.Buildings.Add(building);
        db.Resources.AddRange(resources);
        
        db.PersonalLogs.Add(new()
        {
            User = user,
            Message = "You ascended to godhood.",
            PersonalLogTags = new()
            {
                new PersonalLogTag() { Tag = PersonalLogActivityType.AccountActivity },
                new PersonalLogTag() { Tag = PersonalLogActivityType.AccountCreated }
            }
        });

        await db.SaveChangesAsync(cToken);

        return new ApiResponse();
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed record Request(string VassalName, string Portrait, string PersonalName, string Email, string Passphrase)
    {
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                Transform(x => x.VassalName, n => n.Trim())
                    .NotEmpty().WithMessage("Must provide a name for your prophet.")
                    .MaximumLength(30).WithMessage("Your prophet's name must be 30 characters or less.")
                ;
                
                RuleFor(x => x.Portrait).Must(p => AvailablePortraits.Contains(p)).WithMessage("Must select a portrait for your prophet.");
                
                Transform(x => x.PersonalName, n => n.Trim())
                    .NotEmpty().WithMessage("Must provide a name for yourself.")
                    .MaximumLength(20).WithMessage("Your name must be 20 characters or less.")
                ;
                
                Transform(x => x.Email, n => n.Trim())
                    .NotEmpty().EmailAddress().WithMessage("Must provide an email address.")
                    .MaximumLength(100).WithMessage("Your email address must be 100 characters or less.")
                ;
                
                RuleFor(x => x.Passphrase).MinimumLength(10).WithMessage("Passphrase must be at least 10 characters.");
            }
        }
    }
}