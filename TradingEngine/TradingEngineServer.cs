using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Runtime.CompilerServices;
using TradingEngineServer.Core.Configuration;
using TradingEngineServer.Logging;

namespace TradingEngineServer.Core
{
    sealed class TradingEngineServer : BackgroundService, ITradingEngineServer
    {
        private readonly ITextLogger _logger;
        private readonly IOptions<TradingEngineServerConfiguration> _engineConfiguration;


        public TradingEngineServer(ITextLogger textLogger, 
            IOptions<TradingEngineServerConfiguration> engineConfiguration)
        {
            _engineConfiguration = engineConfiguration ?? throw new ArgumentNullException(nameof(_engineConfiguration));
            _logger = textLogger ?? throw new ArgumentNullException(nameof(textLogger));
            
        }

        public Task Run(CancellationToken token) => ExecuteAsync(token);
  

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Information(nameof(TradingEngineServer), "Starting Trading Engine");
            while (!stoppingToken.IsCancellationRequested)
            {
        
            }
            _logger.Information(nameof(TradingEngineServer), "Stopping Trading Engine");
            return Task.CompletedTask;
        }
    }
}
