using ContestPark.Core.Enums;
using ContestPark.Identity.API.Models;
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

        public async Task SeedAsync(ApplicationDbContext context,
                                    IHostingEnvironment env,
                                    ILogger<ApplicationDbContextSeed> logger,
                                    IOptions<IdentitySettings> settings,
                                    int? retry = 0)
        {
            int retryForAvaiability = retry.Value;

            try
            {
                if (!context.Roles.Any())
                {
                    context.Roles.AddRange(GetRoles());

                    await context.SaveChangesAsync();
                }

                if (!context.Users.Any())
                {
                    context.Users.AddRange(GetDefaultUser());

                    await context.SaveChangesAsync();
                }

                var users = GetDefaultUser();
                foreach (var user in users)
                {
                    if (!context.UserRoles.Any(x => x.UserId == user.Id))
                    {
                        context.UserRoles.Add(new IdentityUserRole<string>
                        {
                            UserId = user.Id,
                            RoleId = "user"
                        });

                        await context.SaveChangesAsync();
                    }
                    if (user.Id == "1111-1111-1111-1111")
                    {
                        context.UserRoles.Add(new IdentityUserRole<string>
                        {
                            UserId = user.Id,
                            RoleId = "admin"
                        });

                        await context.SaveChangesAsync();
                    }
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
            // Buraya user ekleyince diğer servislerede eklenmeli
            var witcherUser =
            new ApplicationUser()
            {
                Id = "1111-1111-1111-1111",
                ProfilePicturePath = "http://i.pravatar.cc/150?u=witcherfearless",
                FullName = "Ali Aldemir",
                UserName = "witcherfearless",
                Email = "aldemirali93@gmail.com",
                PhoneNumber = "1234567890",
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
                Id = "2222-2222-2222-2222",
                ProfilePicturePath = "http://i.pravatar.cc/150?u=demo",
                FullName = "Demo",
                UserName = "demo",
                PhoneNumber = "1234567890",
                NormalizedUserName = "DEMO",
                Email = "demo@demo.com",
                NormalizedEmail = "DEMO@DEMO.COM",
                Language = Languages.English,
                LanguageCode = "tr_TR",
                SecurityStamp = Guid.NewGuid().ToString("D"),
            };

            demoUser.PasswordHash = _passwordHasher.HashPassword(demoUser, "demodemo");

            var botUser =
            new ApplicationUser()
            {
                Id = "3333-3333-3333-bot",
                ProfilePicturePath = "http://i.pravatar.cc/150?u=bot",
                FullName = "Bot",
                UserName = "bot12345",
                PhoneNumber = "1234567890",
                NormalizedUserName = "BOT12345",
                Email = "bot@bot.com",
                NormalizedEmail = "BOT@BOT.COM",
                Language = Languages.Turkish,
                LanguageCode = "en_US",
                IsBot = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
            };

            botUser.PasswordHash = _passwordHasher.HashPassword(botUser, "12345678");

            return new List<ApplicationUser>()
            {
               witcherUser,
               demoUser,
               botUser
            };
        }

        private List<IdentityRole> GetRoles()
        {
            IdentityRole adminRole = new IdentityRole
            {
                Id = "admin",
                Name = "Admin",
                NormalizedName = "ADMIN"
            };
            IdentityRole userRole = new IdentityRole
            {
                Id = "user",
                Name = "User",
                NormalizedName = "USER"
            };

            return new List<IdentityRole>
            {
                adminRole,
                userRole
            };
        }
    }
}