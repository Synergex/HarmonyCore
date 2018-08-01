// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using IdentityServer.Data;
using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer
{
    public class SeedData
    {
        public static void EnsureSeedData(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                context.Database.Migrate();

                //Get the user manager service
                var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                //Create user "jodah"
                var jodah = userMgr.FindByNameAsync("jodah").Result;
                if (jodah == null)
                {
                    jodah = new ApplicationUser { UserName = "jodah" };
                    var result = userMgr.CreateAsync(jodah, "veloper").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    result = userMgr.AddClaimsAsync(jodah, new Claim[]
                    {
                        new Claim(JwtClaimTypes.Name, "Jodah Veloper"),
                        new Claim(JwtClaimTypes.GivenName, "Jodah"),
                        new Claim(JwtClaimTypes.FamilyName, "Veloper"),
                        new Claim(JwtClaimTypes.Email, "jadah.veloper@synergexpsg.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "http://www.jodahveloper.com"),
                        new Claim(JwtClaimTypes.Address, @"{ 'street_address': '2330 Gold Meadow Way', 'locality': 'Gold River', 'postal_code': 95670, 'country': 'United States of America' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json)
                    }).Result;

                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    Console.WriteLine("jodah created");
                }

                //Create user "bigbah"
                var bigbah = userMgr.FindByNameAsync("bigbah").Result;
                if (bigbah == null)
                {
                    bigbah = new ApplicationUser { UserName = "bigbah" };
                    var result = userMgr.CreateAsync(bigbah, "smann").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    result = userMgr.AddClaimsAsync(bigbah, new Claim[]
                    {
                        new Claim(JwtClaimTypes.Name, "Bigbah Smann"),
                        new Claim(JwtClaimTypes.GivenName, "Bigbah"),
                        new Claim(JwtClaimTypes.FamilyName, "Smann"),
                        new Claim(JwtClaimTypes.Email, "bigbah.smann@synergexpsg.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "http://www.jodahveloper.com/meet-bigbah-smann"),
                        new Claim(JwtClaimTypes.Address, @"{ 'street_address': '2330 Gold Meadow Way', 'locality': 'Gold River', 'postal_code': 95670, 'country': 'United States of America' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json)
                    }).Result;

                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    Console.WriteLine("bigbah created");
                }

                //Create user "vi"
                var vi = userMgr.FindByNameAsync("vi").Result;
                if (vi == null)
                {
                    vi = new ApplicationUser { UserName = "vi" };
                    var result = userMgr.CreateAsync(vi, "sprezz").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    result = userMgr.AddClaimsAsync(vi, new Claim[]
                    {
                        new Claim(JwtClaimTypes.Name, "Vi Sprezz"),
                        new Claim(JwtClaimTypes.GivenName, "Vi"),
                        new Claim(JwtClaimTypes.FamilyName, "Sprezz"),
                        new Claim(JwtClaimTypes.Email, "vi.sprezz@synergexpsg.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "http://www.jodahveloper.com/meet-vi-sprezz"),
                        new Claim(JwtClaimTypes.Address, @"{ 'street_address': '2330 Gold Meadow Way', 'locality': 'Gold River', 'postal_code': 95670, 'country': 'United States of America' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json)
                    }).Result;

                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    Console.WriteLine("vi created");
                }

                //Create user "manny"
                var manny = userMgr.FindByNameAsync("manny").Result;
                if (manny == null)
                {
                    manny = new ApplicationUser { UserName = "manny" };
                    var result = userMgr.CreateAsync(manny, "jurr").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    result = userMgr.AddClaimsAsync(manny, new Claim[]
                    {
                        new Claim(JwtClaimTypes.Name, "Manny Jurr"),
                        new Claim(JwtClaimTypes.GivenName, "Manny"),
                        new Claim(JwtClaimTypes.FamilyName, "Jurr"),
                        new Claim(JwtClaimTypes.Email, "manny.jurr@synergexpsg.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "http://www.jodahveloper.com/meet-manny-jurr"),
                        new Claim(JwtClaimTypes.Address, @"{ 'street_address': '2330 Gold Meadow Way', 'locality': 'Gold River', 'postal_code': 95670, 'country': 'United States of America' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json)
                    }).Result;

                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    Console.WriteLine("manny created");
                }

                //Create user "mark"
                var mark = userMgr.FindByNameAsync("mark").Result;
                if (mark == null)
                {
                    mark = new ApplicationUser { UserName = "mark" };
                    var result = userMgr.CreateAsync(mark, "etting").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    result = userMgr.AddClaimsAsync(mark, new Claim[]
                    {
                        new Claim(JwtClaimTypes.Name, "Mark Etting"),
                        new Claim(JwtClaimTypes.GivenName, "Mark"),
                        new Claim(JwtClaimTypes.FamilyName, "Etting"),
                        new Claim(JwtClaimTypes.Email, "mark.etting@synergexpsg.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "http://www.jodahveloper.com/meet-mark-etting"),
                        new Claim(JwtClaimTypes.Address, @"{ 'street_address': '2330 Gold Meadow Way', 'locality': 'Gold River', 'postal_code': 95670, 'country': 'United States of America' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json)
                    }).Result;

                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    Console.WriteLine("mark created");
                }

                //Create user "connie"
                var connie = userMgr.FindByNameAsync("connie").Result;
                if (connie == null)
                {
                    connie = new ApplicationUser { UserName = "connie" };
                    var result = userMgr.CreateAsync(connie, "sultant").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    result = userMgr.AddClaimsAsync(connie, new Claim[]
                    {
                        new Claim(JwtClaimTypes.Name, "Connie Sultant"),
                        new Claim(JwtClaimTypes.GivenName, "Connie"),
                        new Claim(JwtClaimTypes.FamilyName, "Sultant"),
                        new Claim(JwtClaimTypes.Email, "connie.sultant@synergexpsg.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "http://www.jodahveloper.com/meet-connie-sultant"),
                        new Claim(JwtClaimTypes.Address, @"{ 'street_address': '2330 Gold Meadow Way', 'locality': 'Gold River', 'postal_code': 95670, 'country': 'United States of America' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json)
                    }).Result;

                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    Console.WriteLine("connie created");
                }

            }
        }
    }
}
