using Microsoft.EntityFrameworkCore;
using RecAll.Contrib.MaskedTextList.Api.Services;
using RecAll.Core.List.Domain.AggregateModels;
using RecAll.Infrastructure.EventBus.Abstractions;

namespace RecAll.Contrib.MaskedTextList.Api.IntegrationEvents; 

public class ItemIdAssignedIntegrationEventHandler :
    IIntegrationEventHandler<ItemIdAssignedIntegrationEvent> {
    private readonly MaskedTextListContext _MaskedTextListContext;
    private readonly ILogger<ItemIdAssignedIntegrationEventHandler> _logger;

    public ItemIdAssignedIntegrationEventHandler(
        MaskedTextListContext MaskedTextListContext,
        ILogger<ItemIdAssignedIntegrationEventHandler> logger) {
        _MaskedTextListContext = MaskedTextListContext ??
            throw new ArgumentNullException(nameof(MaskedTextListContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(ItemIdAssignedIntegrationEvent @event) {
        if (!ListType.Contains(@event.TypeId)) {
            return;
        }

        _logger.LogInformation(
            "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
            @event.Id, InitialFunctions.AppName, @event);

        var textItem = await _MaskedTextListContext.TextItems.FirstOrDefaultAsync(p =>
            p.Id == int.Parse(@event.ContribId));

        if (textItem is null) {
            _logger.LogWarning("Unknown TextItem id: {ItemId}", @event.ItemId);
            return;
        }

        textItem.ItemId = @event.ItemId;
        await _MaskedTextListContext.SaveChangesAsync();

        _logger.LogInformation(
            "----- Integration event handled: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
            @event.Id, InitialFunctions.AppName, @event);
    }
}