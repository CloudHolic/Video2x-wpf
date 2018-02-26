using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Video2x_wpf.Processor
{
    public struct ThreadStruct
    {
        public string FileName { get; set; }
        public string Argument { get; set; }
        public int Index { get; set; }
    }

    public class WorkerThread
    {
        private static volatile WorkerThread _instance;
        private static readonly object Lock = new object();
        private List<ManualResetEvent> _doneEvents;
        private bool _isCalledByPool;

        public bool IsWorking { get; private set; }
        public bool IsErrorOccurred { get; set; }

        public static WorkerThread Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Lock)
                    {
                        if(_instance == null)
                            _instance = new WorkerThread();
                    }
                }

                return _instance;
            }
        }
        
        private WorkerThread()
        {
            _doneEvents = new List<ManualResetEvent>();
            _isCalledByPool = false;
        }

        public void StartWorker(ThreadStruct info)
        {
            _isCalledByPool = false;
            var thread = new Thread(Worker);
            thread.Start(info);
            thread.Join();
        }

        public void StartWorkerPool(List<ThreadStruct> infos)
        {
            _isCalledByPool = true;
            foreach (var curInfo in infos)
            {
                _doneEvents.Add(new ManualResetEvent(false));
                ThreadPool.QueueUserWorkItem(Worker, curInfo);
            }

            // ReSharper disable once CoVariantArrayConversion
            WaitHandle.WaitAll(_doneEvents.ToArray());
            _doneEvents.Clear();
        }

        private void Worker(object threadInfo)
        {
            IsWorking = true;
            var info = (ThreadStruct) threadInfo;

            try
            {
                var psInfo = new ProcessStartInfo(info.FileName)
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Ref"),
                    Arguments = info.Argument
                };

                var process = Process.Start(psInfo);
                process?.WaitForExit();
            }
            catch
            {
                IsErrorOccurred = true;
            }
            finally
            {
                if (_isCalledByPool)
                    _doneEvents[info.Index].Set();
            }
        }
    }
}
