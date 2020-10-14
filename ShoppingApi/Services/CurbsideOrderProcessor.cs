﻿using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Serilog.Core;
using ShoppingApi.Domain;
using ShoppingApi.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingApi.Services
{
    public class CurbsideOrderProcessor : BackgroundService
    {

        private readonly ILogger<CurbsideOrderProcessor> _logger;
        private readonly CurbsideChannel _channel;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMapper _mapper;
        private readonly IHubContext<CurbsideOrdersHub> _hub;

        public CurbsideOrderProcessor(ILogger<CurbsideOrderProcessor> logger, CurbsideChannel channel, IServiceProvider serviceProvider, IMapper mapper, IHubContext<CurbsideOrdersHub> hub)
        {
            _logger = logger;
            _channel = channel;
            _serviceProvider = serviceProvider;
            _mapper = mapper;
            _hub = hub;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach(var order in _channel.ReadAllAsync())
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ShoppingDataContext>();

                var savedOrder = await context.CurbsideOrders.SingleOrDefaultAsync(c => c.Id == order.OrderId);
                // what if there is no order?
                var items = savedOrder.Items.Split(',').Count();
                for(var t= 0; t< items; t++)
                {
                    await Task.Delay(300); // do all the work for that item...
                    if(order.ClientId != null)
                    {
                        await _hub.Clients.Client(order.ClientId).SendAsync("ItemProcessed", new { message = $"Processed item {t + 1}" });
                    }
                    
                }
                savedOrder.Status = CurbsideOrderStatus.Approved;
                await context.SaveChangesAsync();
                if(order.ClientId !=null)
                {
                    await _hub.Clients.Client(order.ClientId).SendAsync("OrderProcessed", savedOrder);
                }

            }
        }
    }
}
