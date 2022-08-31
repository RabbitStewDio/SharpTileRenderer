using Microsoft.Extensions.Configuration;
using Serilog;
using System;

namespace SharpTileRenderer.Strategy.MonoGame
{
    public static class Program
    {
        static void SetUpLogging()
        {
            var configuration = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json", true)
                                .Build();

            var logger = new LoggerConfiguration()
                         .ReadFrom.Configuration(configuration)
                         .CreateLogger();
            Log.Logger = logger;
        }


        [STAThread]
        static void Main()
        {
            SetUpLogging();

            using var game = new SimpleGame();
            game.Run();
        }
    }
}