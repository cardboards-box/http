using CardboardBox;
using CardboardBox.Http;
using CardboardBox.Http.Tests.Cli;
using CardboardBox.Json;
using Microsoft.Extensions.DependencyInjection;

var code = await new ServiceCollection()
    .AddSerilog()
    .AddCardboardHttp()
    .AddJson()
    .AddTransient<Runner>()
    .BuildServiceProvider()
    .GetRequiredService<Runner>()
    .Run();

Console.WriteLine("Finished - Press any key to exit");
Console.ReadKey();
return code;