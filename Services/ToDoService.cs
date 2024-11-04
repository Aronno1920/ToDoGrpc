using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using ToDoGrpc.Data;
using ToDoGrpc.Models;

namespace ToDoGrpc.Services;

public class ToDoService : ToDoIt.ToDoItBase
{
    private readonly AppDbContext _dbContext;

    public ToDoService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

public override async Task<CreateToDoResponse> CreateToDo (CreateToDoRequest request, ServerCallContext context){
    if(String.IsNullOrWhiteSpace(request.Title)){
        throw new RpcException(new Status(StatusCode.InvalidArgument,"Nothing to save"));
    }

    var todoItem = new ToDoItem{
        Title = request.Title,
        Description = request.Description
    };

    await _dbContext.AddAsync(todoItem);
    await _dbContext.SaveChangesAsync();

    return await Task.FromResult(new CreateToDoResponse{
        Id = todoItem.Id
    });
}

public override async Task<ReadToDoResponse> ReadToDo (ReadToDoRequest request, ServerCallContext context){
    if(String.IsNullOrWhiteSpace(request.Id.ToString())){
        throw new RpcException(new Status(StatusCode.InvalidArgument,"Must provide Todo Id"));
    }

    var todoItem = await _dbContext.toDoItems.FirstOrDefaultAsync(t => t.Id == request.Id);

    if(todoItem != null){
        return await Task.FromResult(new ReadToDoResponse{
            Id = todoItem.Id,
            Title = todoItem.Title,
            Description = todoItem.Description,
            Status = todoItem.Status
        });
    }

    throw new RpcException(new Status(StatusCode.NotFound,"No found with the Id"));
}

public override async Task<GetAllResponse> ReadAllToDo (GetAllRequest request, ServerCallContext context){
    var response = new GetAllResponse();
    var todoItems = await _dbContext.toDoItems.ToListAsync();

    foreach(var item in todoItems){
        response.ToDo.Add(new ReadToDoResponse{
            Id = item.Id,
            Title = item.Title,
            Description = item.Description,
            Status = item.Status
        });
    }

    return await Task.FromResult(response);
}

public override async Task<UpdateToDoResponse> UpdateToDo (UpdateToDoRequest request, ServerCallContext context){
    if(String.IsNullOrWhiteSpace(request.Id.ToString())){
        throw new RpcException(new Status(StatusCode.InvalidArgument,"Must provide Todo Id"));
    }

    var existingItem = await _dbContext.toDoItems.FirstOrDefaultAsync(t => t.Id==request.Id);
    if(existingItem == null){
        throw new RpcException(new Status(StatusCode.NotFound,"No Todo found with the Id"));
    }

    existingItem.Title = request.Title;
    existingItem.Description=request.Description;
    existingItem.Status=request.Status;

    await _dbContext.SaveChangesAsync();

    return await Task.FromResult(new UpdateToDoResponse{
        Id= existingItem.Id
    });
}

public override async Task<DeleteToDoResponse> DeleteToDo (DeleteToDoRequest request, ServerCallContext context){
    if(String.IsNullOrWhiteSpace(request.Id.ToString())){
        throw new RpcException(new Status(StatusCode.InvalidArgument,"Must provide Todo Id"));
    }

    var existingItem = await _dbContext.toDoItems.FirstOrDefaultAsync(t => t.Id==request.Id);
    if(existingItem == null){
        throw new RpcException(new Status(StatusCode.NotFound,"No Todo found with the Id"));
    }

    _dbContext.Remove(existingItem);
    await _dbContext.SaveChangesAsync();

    return await Task.FromResult(new DeleteToDoResponse{
        Id= existingItem.Id
    });
}

}