using Application.Common.Models;
using Application.Features.TodoItems.Commands.AssignUserToTodoItemCommand;
using Application.Features.TodoItems.Commands.CreateTodoItem;
using Application.Features.TodoItems.Commands.DeleteTodoItem;
using Application.Features.TodoItems.Commands.SetTodoItemAsDone;
using Application.Features.TodoItems.Commands.UpdateTodoItemDetail;
using Application.Features.TodoItems.Queries.GetDoneTodoItemsWithPagination;
using Application.Features.TodoItems.Queries.GetTodoItemDetails;
using Application.Features.TodoItems.Queries.GetTodoItems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[Route("api/[controller]/[action]")]
[ApiController]
public class TodoItemsController : ApiControllerBase
{
    /// <summary>
    ///     Assign user to todo item
    /// </summary>
    /// <param name="command">AssignUserToTodoItemCommand</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<int>> AssignUserToTodoItem(AssignUserToTodoItemCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    ///     Creates new TodoItem in specified Group
    /// </summary>
    /// <param name="command">CreateTodoItemCommand</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateTodoItemCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    ///     Deletes TodoItem from specified group
    /// </summary>
    /// <param name="command">DeleteTodoItemCommand</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult> Delete(DeleteTodoItemCommand command)
    {
        await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    ///     Sets specified TodoItem as done
    /// </summary>
    /// <param name="command">SetTodoItemAsDoneCommand</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult> SetTodoItemAsDone(SetTodoItemAsDoneCommand command)
    {
        await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    ///     Updates specified TodoItem
    /// </summary>
    /// <param name="command">UpdateTodoItemDetailCommand</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult> UpdateItemDetails(UpdateTodoItemDetailCommand command)
    {
        await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    ///     Gets paginated list of done TodoItems for specified Group
    /// </summary>
    /// <param name="query">GetDoneTodoItemsWithPaginationQuery</param>
    /// <returns>List of done TodoItems</returns>
    [HttpGet]
    public async Task<ActionResult<PaginatedList<TodoItemBriefDto>>> GetDoneTodoItems(
        [FromQuery] GetDoneTodoItemsWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    /// <summary>
    ///     Gets list of TodoItems that are not done yet for specified Group
    /// </summary>
    /// <param name="query">GetTodoItemsQuery</param>
    /// <returns>List of TodoItems</returns>
    [HttpGet]
    public async Task<ActionResult<List<TodoItemBriefDto>>> GetTodoItems(
        [FromQuery] GetTodoItemsQuery query)
    {
        return await Mediator.Send(query);
    }

    /// <summary>
    ///     Gets details of specific todo item
    /// </summary>
    /// <param name="query">GetTodoItemDetailsQuery</param>
    /// <returns>List of TodoItems</returns>
    [HttpGet]
    public async Task<ActionResult<TodoItemDetailsDto>> GetTodoItemDetails(
        [FromQuery] GetTodoItemDetailsQuery query)
    {
        return await Mediator.Send(query);
    }
}