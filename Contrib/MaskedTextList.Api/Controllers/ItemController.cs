﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecAll.Contrib.MaskedTextList.Api.Commands;
using RecAll.Contrib.MaskedTextList.Api.Models;
using RecAll.Contrib.MaskedTextList.Api.Services;
using RecAll.Contrib.MaskedTextList.Api.ViewModels;
using RecAll.Infrastructure;
using RecAll.Infrastructure.Api;

namespace RecAll.Contrib.MaskedTextList.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ItemController {
    private readonly MaskedTextListContext _MaskedTextListContext;
    private readonly IIdentityService _identityService;
    private readonly ILogger<ItemController> _logger;

    public ItemController(MaskedTextListContext MaskedTextListContext,
        IIdentityService identityService, ILogger<ItemController> logger) {
        _MaskedTextListContext = MaskedTextListContext ??
            throw new ArgumentNullException(nameof(MaskedTextListContext));
        _identityService = identityService ??
            throw new ArgumentNullException(nameof(identityService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [Route("create")]
    [HttpPost]
    public async Task<ActionResult<ServiceResultViewModel<string>>> CreateAsync(
        [FromBody] CreateTextItemCommand command) {
        _logger.LogInformation(
            "----- Handling command {CommandName} ({@Command})",
            command.GetType().Name, command);

        var textItem = new TextItem {
            Content = command.Content,
            UserIdentityGuid = _identityService.GetUserIdentityGuid(),
            IsDeleted = false,
            MaskedContent = command.MaskedContent,
            IsHidden = false
        };
        var textItemEntity = _MaskedTextListContext.Add(textItem);
        await _MaskedTextListContext.SaveChangesAsync();

        _logger.LogInformation("----- Command {CommandName} handled",
            command.GetType().Name);

        return ServiceResult<string>
            .CreateSucceededResult(textItemEntity.Entity.Id.ToString())
            .ToServiceResultViewModel();
    }

    [Route("update")]
    [HttpPost]
    public async Task<ActionResult<ServiceResultViewModel>> UpdateAsync(
        [FromBody] UpdateTextItemCommand command) {
        _logger.LogInformation(
            "----- Handling command {CommandName} ({@Command})",
            command.GetType().Name, command);

        var userIdentityGuid = _identityService.GetUserIdentityGuid();

        var textItem = await _MaskedTextListContext.TextItems.FirstOrDefaultAsync(p =>
            p.Id == command.Id && p.UserIdentityGuid == userIdentityGuid &&
            !p.IsDeleted);

        if (textItem is null) {
            _logger.LogWarning(
                $"用户{userIdentityGuid}尝试查看已删除、不存在或不属于自己的TextItem {command.Id}");

            return ServiceResult
                .CreateFailedResult($"Unknown TextItem id: {command.Id}")
                .ToServiceResultViewModel();
        }

        textItem.Content = command.Content;
        textItem.MaskedContent = command.MaskedContent;

        await _MaskedTextListContext.SaveChangesAsync();

        _logger.LogInformation("----- Command {CommandName} handled",
            command.GetType().Name);

        return ServiceResult.CreateSucceededResult().ToServiceResultViewModel();
    }
    
    [Route("updateHidden")]
    [HttpPost]
    public async Task<ActionResult<ServiceResultViewModel>> UpdateHiddenAsync(
        [FromBody] UpdateHiddenCommand command) {
        _logger.LogInformation(
            "----- Handling command {CommandName} ({@Command})",
            command.GetType().Name, command);

        var userIdentityGuid = _identityService.GetUserIdentityGuid();

        var textItem = await _MaskedTextListContext.TextItems.FirstOrDefaultAsync(p =>
            p.Id == command.Id && p.UserIdentityGuid == userIdentityGuid &&
            !p.IsDeleted);

        if (textItem is null) {
            _logger.LogWarning(
                $"用户{userIdentityGuid}尝试查看已删除、不存在或不属于自己的TextItem {command.Id}");

            return ServiceResult
                .CreateFailedResult($"Unknown TextItem id: {command.Id}")
                .ToServiceResultViewModel();
        }

        textItem.IsHidden = !textItem.IsHidden;
        await _MaskedTextListContext.SaveChangesAsync();

        _logger.LogInformation("----- Command {CommandName} handled",
            command.GetType().Name);

        return ServiceResult.CreateSucceededResult().ToServiceResultViewModel();
    }
    [Route("get/{id}")]
    [HttpGet]
    public async Task<ActionResult<ServiceResultViewModel<TextItemViewModel>>>
        GetAsync(int id) {
        var userIdentityGuid = _identityService.GetUserIdentityGuid();

        var textItem = await _MaskedTextListContext.TextItems.FirstOrDefaultAsync(p =>
            p.Id == id && p.UserIdentityGuid == userIdentityGuid &&
            !p.IsDeleted);

        if (textItem is null) {
            _logger.LogWarning(
                $"用户{userIdentityGuid}尝试查看已删除、不存在或不属于自己的TextItem {id}");

            return ServiceResult<TextItemViewModel>
                .CreateFailedResult($"Unknown TextItem id: {id}")
                .ToServiceResultViewModel();
        }

        return textItem is null
            ? ServiceResult<TextItemViewModel>
                .CreateFailedResult($"Unknown TextItem id: {id}")
                .ToServiceResultViewModel()
            : ServiceResult<TextItemViewModel>
                .CreateSucceededResult(new TextItemViewModel {
                    Id = textItem.Id,
                    ItemId = textItem.ItemId,
                    Content = textItem.Content,
                    MaskedContent = textItem.IsHidden ? TextItem.MaskedString : textItem.MaskedContent
                }).ToServiceResultViewModel();
    }

    [Route("getByItemId/{itemId}")]
    [HttpGet]
    public async Task<ActionResult<ServiceResultViewModel<TextItemViewModel>>>
        GetByItemId(int itemId) {
        var userIdentityGuid = _identityService.GetUserIdentityGuid();

        var textItem = await _MaskedTextListContext.TextItems.FirstOrDefaultAsync(p =>
            p.ItemId == itemId && p.UserIdentityGuid == userIdentityGuid &&
            !p.IsDeleted);

        if (textItem is null) {
            _logger.LogWarning(
                $"用户{userIdentityGuid}尝试查看已删除、不存在或不属于自己的TextItem, ItemID: {itemId}");

            return ServiceResult<TextItemViewModel>
                .CreateFailedResult($"Unknown TextItem with ItemID: {itemId}")
                .ToServiceResultViewModel();
        }

        return textItem is null
            ? ServiceResult<TextItemViewModel>
                .CreateFailedResult($"Unknown TextItem with ItemID: {itemId}")
                .ToServiceResultViewModel()
            : ServiceResult<TextItemViewModel>
                .CreateSucceededResult(new TextItemViewModel {
                    Id = textItem.Id,
                    ItemId = textItem.ItemId,
                    Content = textItem.Content,
                    MaskedContent = textItem.IsHidden ? TextItem.MaskedString : textItem.MaskedContent
                }).ToServiceResultViewModel();
    }

    [Route("getItems")]
    [HttpPost]
    public async
        Task<ActionResult<
            ServiceResultViewModel<IEnumerable<TextItemViewModel>>>>
        GetItemsAsync(GetItemsCommand command) {
        var itemIds = command.Ids.ToList();
        var userIdentityGuid = _identityService.GetUserIdentityGuid();

        var textItems = await _MaskedTextListContext.TextItems.Where(p =>
                p.ItemId.HasValue && itemIds.Contains(p.ItemId.Value) &&
                p.UserIdentityGuid == userIdentityGuid && !p.IsDeleted)
            .ToListAsync();

        if (textItems.Count != itemIds.Count) {
            var missingIds = string.Join(",",
                itemIds.Except(textItems.Select(p => p.ItemId.Value))
                    .Select(p => p.ToString()));

            _logger.LogWarning(
                $"用户{userIdentityGuid}尝试查看已删除、不存在或不属于自己的TextItem {missingIds}");

            return ServiceResult<IEnumerable<TextItemViewModel>>
                .CreateFailedResult($"Unknown Item id: {missingIds}")
                .ToServiceResultViewModel();
        }

        textItems.Sort((x, y) =>
            itemIds.IndexOf(x.ItemId.Value) - itemIds.IndexOf(y.ItemId.Value));

        return ServiceResult<IEnumerable<TextItemViewModel>>
            .CreateSucceededResult(textItems.Select(p => new TextItemViewModel {
                Id = p.Id, ItemId = p.ItemId, Content = p.Content, MaskedContent = p.IsHidden ? TextItem.MaskedString : p.MaskedContent
            })).ToServiceResultViewModel();
    }
}