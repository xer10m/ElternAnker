var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.ElternAnker_Api>("api");

builder.Build().Run();
