using MediaAPI.Modules.FileStorage;
using MediaAPI.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IFileStorage, FileStorageServices>();

// Configure services
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 600 * 1024 * 1024; // 600 MB
});
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 600 * 1024 * 1024; // 600 MB
});


var app = builder.Build();

app.MapPost("/upload", async (IFileStorage fileStorage, HttpContext context) =>
{
    var uploadedFiles = context.Request.Form.Files;
    if (uploadedFiles.Count == 0)
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("No files uploaded.");
        return;
    }

    foreach (var file in uploadedFiles)
    {
        if (file.Length > 500 * 1024 * 1024) // 500MB limit check
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync($"File '{file.FileName}' exceeds the limit 500MB.");
            return;
        }

        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream);
            fileStorage.SaveFile(file.FileName, memoryStream.ToArray());
        }
    }

    context.Response.StatusCode = 200;
    await context.Response.WriteAsync("Files uploaded successfully.");
});
app.MapGet("/files", (IFileStorage fileStorage) =>
{
    var files = fileStorage.GetFiles();
    return Results.Ok(files);
});
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    // non-development environments
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

var fileWatcher = new FileSystemWatcher("./MediaFiles");
fileWatcher.EnableRaisingEvents = true;

fileWatcher.Created += (sender, e) =>
{
    Console.WriteLine($"File added: {e.FullPath}");
}; 
fileWatcher.Changed += (sender, e) =>
{
    Console.WriteLine($"File changed: {e.FullPath}");
};


app.Run();
