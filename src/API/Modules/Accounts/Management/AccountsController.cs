using API.Extensions;
using Asp.Versioning;
using Common.Application.Abstractions.Messaging;
using Common.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Modules.Accounts.Application.Accounts;
using Modules.Accounts.Application.Accounts.Create;
using Modules.Accounts.Application.Accounts.Delete;
using Modules.Accounts.Application.Accounts.GetAll;
using Modules.Accounts.Application.Accounts.GetById;
using Modules.Accounts.Application.Accounts.Update;

namespace API.Modules.Accounts.Management;
[ApiController]
[Route("api/v{v:apiVersion}/accounts")]
[ApiVersion(1)]
public class AccountsController : ControllerBase
{
    [MapToApiVersion(1)]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        IQueryHandler<GetAllAccountsQuery, IReadOnlyList<AccountResponse>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetAllAccountsQuery();

        Result<IReadOnlyList<AccountResponse>> result = await handler.Handle(query, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToActionResult();
        }

        return Ok(result.Value);
    }


    [MapToApiVersion(1)]
    [HttpGet("accountId:guid")]
    public async Task<IActionResult> GetById(
        IQueryHandler<GetByIdAccountQuery, AccountResponse> handler,
        Guid accountId,
        CancellationToken cancellationToken)
    {
        var query = new GetByIdAccountQuery(accountId);

        Result<AccountResponse> result = await handler.Handle(query, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToActionResult();
        }

        return Ok(result.Value);
    }

    [MapToApiVersion(1)]
    [HttpPost]
    public async Task<IActionResult> Create(
        ICommandHandler<CreateAccountCommand, Guid> handler,
        CreateAccountRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateAccountCommand(
            request.UserId,
            request.Name,
            request.Type,
            request.BalanceAmount,
            request.BalanceCurrency);

        Result<Guid> result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToActionResult();
        }

        return Ok(result.Value);
    }

    [MapToApiVersion(1)]
    [HttpPut("accountId:guid")]
    public async Task<IActionResult> Update(
        ICommandHandler<UpdateAccountCommand, Guid> handler,
        Guid accountId,
        UpdateAccountRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateAccountCommand(
            accountId,
            request.Name,
            request.Type,
            request.BalanceAmount,
            request.BalanceCurrency);

        Result<Guid> result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToActionResult();
        }

        return Ok(result.Value);
    }


    [MapToApiVersion(1)]
    [HttpDelete("accountId:guid")]
    public async Task<IActionResult> Delete(
        ICommandHandler<DeleteAccountCommand, Guid> handler,
        Guid accountId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteAccountCommand(accountId);

        Result<Guid> result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToActionResult();
        }

        return Ok(result.Value);
    }
}
