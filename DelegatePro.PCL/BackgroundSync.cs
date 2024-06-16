using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DelegatePro.PCL
{
    public class BackgroundSync
    {
        private static TimeSpan SyncInterval = TimeSpan.FromSeconds(Constants.SyncIntervalSeconds);

        private static CancellationTokenSource _cancelToken = new CancellationTokenSource();

        public static async void Start()
        {
            if (!_cancelToken.IsCancellationRequested)
            {
                _cancelToken.Cancel();
                _cancelToken = new CancellationTokenSource();
                await Task.Delay(2000);
            }

            Debug.WriteLine("======== Starting background sync =========");
            SyncData();

            Task.Delay(SyncInterval, _cancelToken.Token).ContinueWith(task =>
            {
                if (!task.IsCanceled)
                {
                    Start();    
                }
                else
                {
                    Debug.WriteLine("=========== Cancelled background sync ============");
                }

            }).WithoutAwait();
        }

        private static void SyncData()
        {
            Case.UploadCasesAsync().WithoutAwait();
            POI.UploadAsync().WithoutAwait();
            Note.UploadNotesAsync().WithoutAwait();
        }

        public static void Stop()
        {
            _cancelToken.Cancel();
        }
    }
}