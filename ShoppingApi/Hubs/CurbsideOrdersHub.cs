﻿using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using ShoppingApi.Domain;
using ShoppingApi.Models.Curbside;
using ShoppingApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingApi.Hubs
{
    public class CurbsideOrdersHub : Hub
    {
        private IDoCurbsideCommands _commands;
        private readonly ILogger<CurbsideOrdersHub> _logger;
        private readonly IMapper _mapper;
        private readonly CurbsideChannel _channel;

        public CurbsideOrdersHub(IDoCurbsideCommands commands, ILogger<CurbsideOrdersHub> logger, IMapper mapper, CurbsideChannel channel)
        {
            _commands = commands;
            _logger = logger;
            _mapper = mapper;
            _channel = channel;
        }
        public override Task OnConnectedAsync()
        {
            
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        public async Task PlaceOrder(PostCurbsideOrderRequest orderToBePlaced)
        {
            CurbsideOrder order = await _commands.AddOrderWs(orderToBePlaced, Context.ConnectionId);
            
            await Clients.Caller.SendAsync("OrderPlaced", order);
        }
    }
}
