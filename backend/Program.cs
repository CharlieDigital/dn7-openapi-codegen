var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

// Set up the OpenAPI spec generation on build.
builder.Services.AddSwaggerGen(spec => {
  var filePath = Path.Combine(AppContext.BaseDirectory, "backend.xml");

  spec.IncludeXmlComments(filePath);
  spec.DescribeAllParametersInCamelCase();
  spec.SupportNonNullableReferenceTypes();
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseSwagger(); // Use swagger
app.MapControllers();

app.Run();
