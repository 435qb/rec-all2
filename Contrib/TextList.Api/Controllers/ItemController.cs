using Microsoft.AspNetCore.Mvc;
using RecAll.Contrib.TextList.Api.Commands;
using RecAll.Contrib.TextList.Api.Models;
using RecAll.Contrib.TextList.Api.Services;

namespace RecAll.Contrib.TextList.Api.Controllers;

[Route("[controller]")]
public class ItemController {
    private readonly TextListContext _textListContext;
    private readonly IIdentityService _identityService;

    public ItemController(TextListContext textListContext,
        IIdentityService identityService) {
        _textListContext = textListContext;
        _identityService = identityService;
    }
    
    [Route("create")]
    [HttpPost]
    public async Task<ActionResult<string>> CreateAsync(
        [FromBody] CreateTextItemCommand command) {
        var textItem = new TextItem {
            Content = command.Content,
            UserIdentityGuid = _identityService.GetUserIdentityGuid(),
            IsDeleted = false
        };
        var textItemEntity = _textListContext.Add(textItem);
        await _textListContext.SaveChangesAsync();

        return textItemEntity.Entity.Id.ToString();
    }
}