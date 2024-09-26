using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using WebAPIwithMongoDB.Services;

namespace WebAPIwithMongoDB.Controllers
{
    public class PointsResetHostedService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        // Loại bỏ dependency trực tiếp tới PointsResetService
        public PointsResetHostedService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(ResetPoints, null, TimeSpan.Zero, TimeSpan.FromDays(1)); // Gọi mỗi ngày
            return Task.CompletedTask;
        }

        private async void ResetPoints(object state)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var pointsResetService = scope.ServiceProvider.GetRequiredService<PointsResetService>();
                await pointsResetService.ResetPoints(); // Sử dụng scoped PointsResetService trong scope mới
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
