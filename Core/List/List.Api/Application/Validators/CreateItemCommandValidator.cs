using FluentValidation;
using RecAll.Core.List.Api.Application.Commands;
using RecAll.Core.List.Api.Infrastructure.Services;
using RecAll.Core.List.Domain.AggregateModels;
using RecAll.Core.List.Domain.AggregateModels.SetAggregate;
using RecAll.Infrastructure.Ddd.Domain.SeedWork;

namespace RecAll.Core.List.Api.Application.Validators;

public class CreateItemCommandValidator : AbstractValidator<CreateItemCommand>
{
    public CreateItemCommandValidator(
        IIdentityService identityService,
        ISetRepository setRepository,
        ILogger<CreateItemCommandValidator> logger)
    {
        RuleFor(p => p.SetId).NotEmpty();
        RuleFor(p => p.CreateContribJson).NotEmpty();
        RuleFor(p => p.SetId).MustAsync(async (p, _) =>
        {
            var userIdentityGuid = identityService.GetUserIdentityGuid();
            var isValid =
                await setRepository.GetAsync(p, userIdentityGuid) is not null;

            if (!isValid)
            {
                logger.LogWarning(
                    $"用户{userIdentityGuid}尝试在已删除、不存在或不属于自己的Set {p}中创建item");
            }

            return isValid;
        }).WithMessage("无效的Set ID");
        RuleFor(p => p).MustAsync(async (p, _) =>
        {
            var userIdentityGuid = identityService.GetUserIdentityGuid();
            var set = await setRepository.GetAsync(p.SetId, userIdentityGuid);
            var id = set.Type.Id;
            var isValid = id switch
            {
                ListType.TextId => p.CreateContribJson.ContainsKey("content"),
                ListType.MaskedTextId => p.CreateContribJson.ContainsKey("content") &&
                                         p.CreateContribJson.ContainsKey("maskedcontent"),
                _ => false
            };
            if (!isValid)
            {
                logger.LogWarning(
                    $"用户{userIdentityGuid}创建item时格式错误");
            }

            return isValid;
        }).WithMessage("Json格式有误").WhenAsync(async (p, _) =>
        {
            var userIdentityGuid = identityService.GetUserIdentityGuid();
            return await setRepository.GetAsync(p.SetId, userIdentityGuid) is not null;
        });
        logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
    }
}