using Castle.Auth;
using Castle.Helpers;
using Castle.Interfaces;
using Castle.Models.C2Profile;
using Castle.Models.Commander;
using Castle.Models.Raven;
using Castle.Models.Knight;
using Castle.Services;
using Microsoft.OpenApi.Models;

namespace Castle
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            string c2Profile = "Default";
            //string c2Profile = "Test";

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            /////////////
            //builder.Services.AddCors();         // DO I NEED THIS
            builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

            builder.Services.AddSingleton<IJwtUtils, JwtUtils>();
            builder.Services.AddSingleton<IUserService, UserService>();
            builder.Services.AddSingleton<IRequestHeader, RequestHeader>();
            builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
            builder.Services.AddSingleton<ICommanderService, CommanderService>();
            builder.Services.AddSingleton<IKnightService, KnightService>();
            builder.Services.AddSingleton<IRavenService, RavenService>();
            builder.Services.AddSingleton<IKnightHistoryService, KnightHistoryService>();
            builder.Services.AddSingleton<IPeerToPeerService, PeerToPeerService>();
            builder.Services.AddSingleton<IC2ProfileService, C2ProfileService>();
            builder.Services.AddSingleton<IHostedFileService, HostedFilesService>();


#if DEBUG //  JWT for Swagger            
            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
                });
            });
#endif

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            ///////////////         //  DO I NEED THESE ???
            //app.UseCors(x => x              
            //    .AllowAnyOrigin()
            //    .AllowAnyMethod()
            //    .AllowAnyHeader());
            ///////////////


            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseMiddleware<JwtMiddleware>();
            

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();


            // SetProfile >> Load C2Profile from yaml file
            var c2ProfileService = (C2ProfileService)app.Services.GetRequiredService<IC2ProfileService>();
            //var result = await c2ProfileService.SetProfile(c2Profile);
            var result = await c2ProfileService.SetProfileHardCoded(c2Profile);
            if (result == null)
            {
                Console.WriteLine($"[!] Failed to set C2Profile for {c2Profile}");
                System.Environment.Exit(1);
            }

            // Load Commanders in DB
            var commanderService = (CommanderService)app.Services.GetRequiredService<ICommanderService>();
            await commanderService.LoadFromDb();
            Console.WriteLine("[$$$] Skipping Commander db load, making default http-1 and http-2");


            #region TEST
            //////////////// DEBUG
            // Soldiers
            //string saveSoldierId = string.Empty;
            //var soldierService = (SoldierService)app.Services.GetRequiredService<ISoldierService>();
            //for (int i = 0; i < 5; i++)
            //{
            //    var soldier = new Soldier
            //    {
            //        Metadata = new SoldierMetadata
            //        {
            //            Id = Guid.NewGuid().ToString(),
            //            Address = "addr-" + i,
            //            Hostname = "host-" + 1,
            //            Integrity = Integrity.MEDIUM,
            //            Username = "user-" + i,
            //            ProcessName = "procname-" + i,
            //            ProcessId = i,
            //            x64 = true
            //        }


            //    };
            //    soldierService.AddSoldier(soldier);
            //    if (i == 0)
            //    {
            //        saveSoldierId = soldier.Metadata.Id;
            //    }
            //}

            //// TaskMessage
            //var historyService = (SoldierHistoryService)app.Services.GetRequiredService<ISoldierHistoryService>();
            //for (int i = 0; i < 5; i++)
            //{
            //    var taskMsg = new TaskMessage
            //    {
            //        TaskId = Guid.NewGuid().ToString(),
            //        Command = "cmd-" + i,
            //        Arguments = new string[] { "arg-" + i },
            //    };
            //    var history = new SoldierHistory(saveSoldierId, taskMsg);
            //    historyService.Add(history);
            //}

            //////////////// 
            #endregion

            #region TEST_CodeBuilder
            // Test building SharedArsenal, MB, Knight using C2Profile + SplitStringSource
            #endregion

            app.Run();

        }
    }
}