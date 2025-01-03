using NetAuthManager.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace NetAuthManager.Application;

public class SystemService : ISystemService, ITransient
{
    private readonly ILogger<SystemService> _logger;

    private readonly IRepository<SysMenu> _menuRepository;
    private readonly IServiceScopeFactory _scopeFactory;

    public SystemService(ILogger<SystemService> logger, IRepository<SysMenu> menuRepository, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;

        _menuRepository = menuRepository;
        _scopeFactory = scopeFactory;
    }

    public string GetDescription()
    {
        return "财务共享服务中心运营平台接口，让财务运营更简单。";
    }
}
