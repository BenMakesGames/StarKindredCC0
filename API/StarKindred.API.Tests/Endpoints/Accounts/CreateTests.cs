using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Endpoints.Accounts;
using StarKindred.Common.Services;
using Xunit;

namespace StarKindred.API.Tests.Endpoints.Accounts;

public class CreateTests
{
    [Fact]
    public async Task Test()
    {
        var sut = new Create();

        var options = new DbContextOptionsBuilder<Db>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var db = new Db(options);

        var validRequest = new Create.Request(
            "Vassal Name",
            Create.AvailablePortraits.First(),
            "Personal Name",
            "e@mail.com",
            "passphrase"
        );

        var passphraseHasher = new PassphraseHasher();

        await sut._(validRequest, db, passphraseHasher, default);

        var user = db.Users.FirstOrDefault();
        var vassal = db.Vassals.FirstOrDefault();
        var town = db.Towns.FirstOrDefault();

        user.Should().NotBeNull();
        user!.Name.Should().Be(validRequest.PersonalName);
        user.Email.Should().Be(validRequest.Email);
        passphraseHasher.Verify(validRequest.Passphrase, user.Passphrase).Should().BeTrue();

        vassal.Should().NotBeNull();
        vassal!.Name.Should().Be(validRequest.VassalName);

        town.Should().NotBeNull();
    }
}