using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Exceptions;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;
using Stripe;
using Stripe.Checkout;

namespace StarKindred.API.Endpoints.Stripe;

[ApiController]
public sealed class Webhook
{
    // TODO: this should be pulled out into appsettings.json
    private const string EndpointSecret = "YOUR_ENDPOINT_SECRET_HERE";

    private static readonly string[] WebhookSenderIPs =
    {
        "127.0.0.1",

        "3.18.12.63",
        "3.130.192.231",
        "13.235.14.237",
        "13.235.122.149",
        "18.211.135.69",
        "35.154.171.200",
        "52.15.183.38",
        "54.88.130.119",
        "54.88.130.237",
        "54.187.174.169",
        "54.187.205.235",
        "54.187.216.72"
    };

    [HttpPost("/stripe/webhook")]
    public async Task Handle(
        [FromServices] Db db,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken cToken
    )
    {
        if (httpContextAccessor.HttpContext is not { } httpContext)
            throw new UnprocessableEntity("Bad request.");

        if (httpContext.Connection.RemoteIpAddress?.ToString() is not { } ipAddress)
            throw new UnprocessableEntity("Bad request.");

        if (!WebhookSenderIPs.Contains(ipAddress))
            throw new UnprocessableEntity("Bad request.");

        var json = await new StreamReader(httpContext.Request.Body).ReadToEndAsync();

        var stripeEvent = EventUtility.ConstructEvent(
            json,
            httpContext.Request.Headers["Stripe-Signature"],
            EndpointSecret
        );

        Console.WriteLine($"Received event: {stripeEvent.Id} - {stripeEvent.Type}");

        // Handle the event
        if (stripeEvent.Type == Events.CheckoutSessionCompleted)
        {
            await HandleCheckoutSessionCompleted(db, stripeEvent.Data.Object as Session, cToken);
        }
        else if (stripeEvent.Type == Events.InvoicePaid)
        {
            await HandleInvoicePaid(db, stripeEvent.Data.Object as Invoice, cToken);
        }
        else
        {
            throw new UnprocessableEntity("Bad request.");
        }

        await db.SaveChangesAsync(cToken);
    }

    private async Task HandleInvoicePaid(Db db, Invoice? invoice, CancellationToken cToken)
    {
        if (invoice == null)
            return;

        var subscriptionId = invoice.SubscriptionId;

        var sub = await db.UserSubscriptions.FirstOrDefaultAsync(
            s => s.SubscriptionService == "Stripe" && s.SubscriptionId == subscriptionId,
            cToken
        );

        if (sub == null)
            return;

        sub.EndDate = DateTimeOffset.UtcNow.AddMonths(1).AddDays(1).ToUniversalTime();
    }

    private async Task HandleCheckoutSessionCompleted(Db db, Session? checkoutSession, CancellationToken cToken)
    {
        if (checkoutSession == null)
            return;

        var userId = Guid.Parse(checkoutSession.ClientReferenceId);
        var subscriptionId = checkoutSession.SubscriptionId;

        var sub = await db.UserSubscriptions.FirstOrDefaultAsync(
            s => s.SubscriptionService == "Stripe" && s.SubscriptionId == subscriptionId,
            cToken
        );

        if (sub != null)
            return;

        sub = new UserSubscription()
        {
            UserId = userId,
            SubscriptionService = "Stripe",
            SubscriptionId = subscriptionId,
            StartDate = DateTimeOffset.UtcNow,
            EndDate = DateTimeOffset.UtcNow.AddMonths(1).AddDays(1).ToUniversalTime()
        };

        db.UserSubscriptions.Add(sub);
    }
}
