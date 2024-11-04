using Grpc.Core;
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




//     rpc CreateToDo(CreateToDoRequest) returns (CreateToDoResponse)
// {

// }

// // Read Single
// rpc ReadToDo(ReadToDoRequest) returns (ReadToDoResponse){

// }

// // Read List
// rpc ReadAllToDo(GetAllRequest) returns (GetAllResponse){
    
// }

// // Update
// rpc UpdateToDo(UpdateToDoRequest) returns (UpdateToDoResponse){

// }

// // Delete
// rpc DeleteToDo(DeleteToDoRequest) returns (DeleteToDoResponse){
    
// }
}