using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Entities;
using WebAPI.Features;



//create a webApplicationbuilder and a webApplication 
//with preconfigured defaults
var builder = WebApplication.CreateBuilder(args);


var conString = builder.Configuration.GetConnectionString("EmployeeDb") ?? "DataSource = TimeSheet.db";
//add the database context to the dependency injection (DI) container
builder.Services.AddSqlite<EmployeeTimeSheetDb>(conString);

/*builder.Services.AddDbContext<EmployeeTimeSheetDb>(options =>
    options.UseInMemoryDatabase("EmployeeList");
});*/

//add swageer Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//with preconfigured defaults
var app = builder.Build();

//Enable the middleware for serving the generated JSON doucment and the swagger UI 
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

//Enable the swagger ui in devlopment mode only

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



//Create a group URL 
var empItems = app.MapGroup("/employees");
empItems.MapGet("/", GetAllEmployee);
empItems.MapGet("/{Id}", GetEmployeeById);
empItems.MapPost("/", CreateEmployee);
empItems.MapPut("/", UpdateEmployee);
empItems.MapDelete("/{Id}", DeleteEmployee);
empItems.MapGet("/Hello", () => "Hi");

/*var employeeLists = new List<Employee>
{
    new Employee() {Id = 1,Name="Jame"},
     new Employee() {Id = 2,Name="Join"},
      new Employee() {Id = 3,Name="Smith"}
};


var empItems = app.MapGroup("/employees");
empItems.MapGet("/", GetEmployeeBy);
//Read all employee
IResult GetEmployeeBy(int id)
{
    var emp = employeeLists.Find(s=> s.Id == id);
    return TypedResults.Ok(employeeLists);
}

//Read all employees by id
app.MapGet("/employees/{id}", (int id) =>
{
    var employee = employeeLists.Find(s => s.Id == id);
    return employee == null ? Results.NotFound() : Results.Ok(employee);
});



// Add new employee
app.MapPost("/employees", ([FromBody] Employee inputEmp) =>
{
    employeeLists.Add(inputEmp);
    return Results.Created($"/employees/{inputEmp.Id}", inputEmp);
});

//Update the employee

app.MapPut("/employees", ([FromBody] Employee inputEmp) =>
{
    if (inputEmp is null) return Results.NotFound();
    int foundIndex = employeeLists.FindIndex(s => s.Id == inputEmp.Id);
    if (foundIndex < 0) return Results.NotFound();
    employeeLists[foundIndex] = inputEmp;
    return Results.NoContent();
});


//Detele the employeee

app.MapDelete("/employees/{id}", (int? id) =>
{
    if (id is null) return Results.NotFound();

    int foundIndex = employeeLists.FindIndex(s => s.Id == id);
    if (foundIndex < 0) return Results.NotFound();

    employeeLists.RemoveAt(foundIndex);
    return Results.Ok(id);
});
*/




//Run the rest api server at port : 7192, change in launchsetting.json

app.Run();


//Read all employees 
static async Task<IResult> GetAllEmployee( EmployeeTimeSheetDb db)
{
    return TypedResults.Ok(await db.Employees.ToArrayAsync());

}

//Read by id
static async Task<IResult> GetEmployeeById(int Id ,EmployeeTimeSheetDb db)
{
    var emp = await db.Employees.FindAsync(Id);
    return emp == null ? TypedResults.NotFound() : TypedResults.Ok(emp);
}

//add new 
static async Task<IResult> CreateEmployee([FromBody] Employee inputEmp,  EmployeeTimeSheetDb db)
{
    db.Employees.Add(inputEmp);
    await db.SaveChangesAsync();
    return TypedResults.Created($"/employees/{inputEmp.Id}", inputEmp);

}


//update emp
static async Task<IResult> UpdateEmployee([FromBody] Employee inputEmp, EmployeeTimeSheetDb db){
    if (inputEmp is null ) return TypedResults.NotFound();

    var emp = await db.Employees.FindAsync(inputEmp.Id);    
    if(emp is null) return TypedResults.NotFound();

    emp.Name = inputEmp.Name;//update the student name
    await db.SaveChangesAsync();
    return TypedResults.NoContent();
}

//delete
static async Task<IResult> DeleteEmployee(int? Id, EmployeeTimeSheetDb db)
{
    if (Id is null) return TypedResults.NotFound();

    var emp = await db.Employees.FindAsync(Id);
    if (emp is null) return TypedResults.NotFound();

    db.Employees.Remove(emp);   
    await db.SaveChangesAsync();
    return TypedResults.Ok(Id);
}