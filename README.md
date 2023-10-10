# Ultimate .NET OpenAPI Workflow

If you're building full-stack TypeScript solutions with .NET or you have a setup where you need to have the same models in some TypeScript code (e.g. same model definitions in a TypeScript serverless function), then you've come to the right place!

I've previously written about [how to set up OpenAPI TypeScript client generation with .NET 6](https://chrlschn.medium.com/net-6-web-apis-with-openapi-typescript-client-generation-a743e7f8e4f5).  But one problem with this approach is that if we want to have that sweet `dotnet watch` hot reload on the API, we can't generate the updated client unless we:

1. Trigger rebuild ("rude edit")
2. Stop the watcher and restart it

Just rebuilding it won't work properly as it would attempt to write the binaries to the same target.

Ideally, we can keep the service running since we're only updating the generated TypeScript client bindings.

For this to work, we'll need a bit of simple -- but perhaps not-so-obvious -- configurations.

## Setting Up the Frontend

On the frontend, we are setting up a basic directory structure.

When setting this up, you would in initialize your application frontend first and then decide where you want the generated API bindings to go.

In our case, our bindings will go into `/app/src/_api`.

Once this is set up, run the following:

```bash
# Add the codegen tooling
cd app
yarn add openapi-typescript-codegen
```

Then set up the codegen script in `package.json`

```json
{
  "name": "app",
  "version": "1.0.0",
  "main": "index.js",
  "license": "MIT",
  "dependencies": {
    "openapi-typescript-codegen": "^0.25.0"
  },
  "scripts": {
    "gen": "npx openapi --input ./src/_api/schema-api.json --output ./src/_api/ --client axios --useUnionTypes"
  }
}
```

## Setting Up the Backend

The backend setup is where it gets interesting.

We want to add two post-build actions:

1. Generate the OpenAPI schema
2. Generate the TypeScript client bindings

Before we do that, we need to initialize the .NET tool manifest and install the Swagger CLI tooling:

```bash
# Initialize the tool manifest
cd backend
dotnet new tool-manifest

# Install Swagger CLI tooling
dotnet tool install swashbuckle.aspnetcore.cli
```

Now that we have the tooling, we need to update the `.csproj` file.  First, to invoke the tool:

```xml
<!--
  Generates the OpenAPI schema file into the front-end API target.
-->
<Target Name="Swagger" AfterTargets="Build" Condition="$(Configuration)=='Gen' Or $(GEN)=='true'">
  <Message Text="Generating OpenAPI schema file." Importance="high" />
  <Exec Command="dotnet tool restore" />
  <!-- Generate the external API.  See SetupSwagger for this doc. -->
  <Exec
    Command="dotnet swagger tofile --output ../app/src/_api/schema-api.json $(OutputPath)$(AssemblyName).dll v1"
    EnvironmentVariables="ASPNETCORE_ENVIRONMENT=Development"
    WorkingDirectory="$(ProjectDir)" />
</Target>
```

There are two special things we've added to this build script:

1. A condition to only run this script if the `Configuration = 'Gen'`
2. Or if the `GEN` environment variable is `true`

These two conditions serve different purposes.  The first is to allow us to have to output directories since each build configuration maps to a different output directory.

Why is this important?  If we want to run `dotnet watch`, that will prevent us from generating the schema and clients unless we stop the application first.  Ideally, we can keep hot reload *and* be able to update the client.

The second condition allows us to include the build in CI/CD if we want to generate it as part of our `Release` build.

Now we add the second post-build action:

```xml
<!--
  Generates the TypeScript clients and models using the schema.
-->
<Target Name="GenTsClient" AfterTargets="Build" Condition="$(Configuration)=='Gen' And $(GEN)=='true'">
  <Message Text="Generating TypeScript client." Importance="high" />
  <!-- Generate TS bindings for the web app -->
  <Exec Command="yarn gen" WorkingDirectory="../app" />
</Target>
```

This simply invokes the `gen` script that was configured in `/app/package.json`.

Finally, in the main `PropertyGroup`, we'll add a declaration to get the XML comments which will be pulled into the client schema:

```xml
<PropertyGroup>
  <TargetFramework>net7.0</TargetFramework>
  <Nullable>enable</Nullable>
  <ImplicitUsings>enable</ImplicitUsings>
  <!-- This gives us comments in our schema -->
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
</PropertyGroup>
```

In our code, we need to set up the Swagger pipeline to pull in the comments:

```csharp
// [Program.cs]
// Set up the OpenAPI spec generation on build.
builder.Services.AddSwaggerGen(spec => {
  var filePath = Path.Combine(AppContext.BaseDirectory, "backend.xml");

  spec.IncludeXmlComments(filePath);
  spec.DescribeAllParametersInCamelCase();
  spec.SupportNonNullableReferenceTypes();
});
```

Now we're ready to roll.

## Using This Configuration

To make it easier to use this configuration, we'll add two scripts to the `backend` for convenience:

```bash
# _watch.sh
dotnet watch --non-interactive

# _gen.sh
dotnet watch build --non-interactive -- --configuration Gen
```

The first is simply a script to initiate hot reload on our API with the `--non-interactive` flag.

The second does the same but instead of hot reloading our code, it generates a new schema instead!

It's easy to write the equivalent batch or PowerShell scripts and you can even script them to run together.

A special note: `dotnet watch build` is broken with the preview version of .NET 8 that I have installed (`8.0.100-preview.7.23376.3`) so the `global.json` file is necessary to point to a stable release version where `dotnet watch build` does work correctly.  To see which versions you have locally, run `dotnet --list-sdks`.