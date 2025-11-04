using Fundo.Applications.WebApiSecurity;
using Microsoft.AspNetCore;

public static class Program
{
    public static void Main(string[] args)
    {
        try
        {
            CreateWebHostBuilder(args).Build().Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unhandled WebApi exception: {ex.Message}");
        }
        finally
        { 
            Console.WriteLine("Application shutting down.");
        }
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args)
    {
        return WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>();
        //.UseUrls("http://0.0.0.0:8080");
    }
}