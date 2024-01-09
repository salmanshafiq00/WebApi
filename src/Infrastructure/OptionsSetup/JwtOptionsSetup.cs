﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace WebApi.Infrastructure.OptionsSetup;

public class JwtOptionsSetup(IConfiguration configuration) 
    : IConfigureOptions<JwtOptions>
{
    public void Configure(JwtOptions options)
    {
        configuration.GetSection(JwtOptions.JWT).Bind(options);
    }
}
