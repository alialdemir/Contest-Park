using ContestPark.Core.Enums;
using ContestPark.Identity.API.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Identity.API.Data
{
    public class ApplicationDbContextSeed
    {
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher = new PasswordHasher<ApplicationUser>();

        public async Task SeedAsync(ApplicationDbContext context, IHostingEnvironment env,
            ILogger<ApplicationDbContextSeed> logger, IOptions<IdentitySettings> settings, int? retry = 0)
        {
            int retryForAvaiability = retry.Value;

            try
            {
                var contentRootPath = env.ContentRootPath;
                var webroot = env.WebRootPath;

                if (!context.Users.Any())
                {
                    context.Users.AddRange(GetDefaultUser());

                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                if (retryForAvaiability < 10)
                {
                    retryForAvaiability++;

                    logger.LogError(ex.Message, $"There is an error migrating data for ApplicationDbContext");

                    await SeedAsync(context, env, logger, settings, retryForAvaiability);
                }
            }
        }

        private IEnumerable<ApplicationUser> GetDefaultUser()
        {
            var witcherUser =
            new ApplicationUser()
            {
                Id = "1111-1111-1111-1111",
                ProfilePicturePath = "http://i.pravatar.cc/150?u=witcherfearless",
                FullName = "Ali Aldemir",
                PhoneNumber = "1234567890",
                UserName = "witcherfearless",
                Email = "aldemirali93@gmail.com",
                NormalizedEmail = "ALDEMIRALI93@GMAIL.COM",
                NormalizedUserName = "WITCHERFEARLESS",
                LanguageCode = "tr_TR",
                Language = Languages.Turkish,
                SecurityStamp = Guid.NewGuid().ToString("D"),
            };

            witcherUser.PasswordHash = _passwordHasher.HashPassword(witcherUser, "19931993");

            var demoUser =
            new ApplicationUser()
            {
                Id = "2222-2222-2222-bot",
                ProfilePicturePath = "http://i.pravatar.cc/150?u=demo",
                FullName = "Demo",
                PhoneNumber = "1234567890",
                UserName = "demo",
                NormalizedUserName = "DEMO",
                Email = "demo@demo.com",
                NormalizedEmail = "DEMO@DEMO.COM",
                Language = Languages.English,
                LanguageCode = "tr_TR",
                SecurityStamp = Guid.NewGuid().ToString("D"),
            };

            demoUser.PasswordHash = _passwordHasher.HashPassword(demoUser, "demo");

            var botUser =
            new ApplicationUser()
            {
                Id = "3333-3333-3333-bot",
                ProfilePicturePath = "http://i.pravatar.cc/150?u=bot",
                FullName = "Bot",
                PhoneNumber = "1234567890",
                UserName = "Bot",
                NormalizedUserName = "BOT",
                Email = "bot@bot.com",
                NormalizedEmail = "BOT@BOT.COM",
                Language = Languages.Turkish,
                LanguageCode = "en_US",
                SecurityStamp = Guid.NewGuid().ToString("D"),
            };

            demoUser.PasswordHash = _passwordHasher.HashPassword(demoUser, "contestparkbot123");

            return new List<ApplicationUser>()
            {
               witcherUser,
               demoUser,
               botUser
            };
        }
    }
}