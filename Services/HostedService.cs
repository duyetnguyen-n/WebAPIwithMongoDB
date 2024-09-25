using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using WebAPIwithMongoDB.Services;
namespace WebAPIwithMongoDB.Controllers
{
    public class PointsResetHostedService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly PointsResetService _pointsResetService;

        public PointsResetHostedService(PointsResetService pointsResetService)
        {
            _pointsResetService = pointsResetService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(ResetPoints, null, TimeSpan.Zero, TimeSpan.FromDays(1)); // Gọi mỗi ngày
            return Task.CompletedTask;
        }

        private async void ResetPoints(object state)
        {
            await _pointsResetService.ResetPoints();
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
