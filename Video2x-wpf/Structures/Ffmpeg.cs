using Video2x_wpf.Processor;

namespace Video2x_wpf.Structures
{
    /// <summary>
    /// Communicates with ffmpeg
    /// </summary>
    public class Ffmpeg
    {
        private readonly WorkerThread _worker;
        private readonly string _filePath;

        public Ffmpeg(string path)
        {
            _worker = WorkerThread.Instance;
            _filePath = path;
        }
        
        /// <summary>
        /// Extract every frame from original videos
        /// </summary>
        /// <param name="input">input video path</param>
        /// <param name="outdir">video output directory</param>
        public void ExtractFrames(string input, string outdir)
        {
            InternalCallFfmpeg($"-i {input} {outdir}/extracted_%0d.png -y");
        }

        /// <summary>
        /// Strips audio tracks from videos
        /// </summary>
        /// <param name="input">input video path</param>
        /// <param name="outdir">video output directory</param>
        public void ExtractAudio(string input, string outdir)
        {
            InternalCallFfmpeg($"-i {input} -vn -acodec copy {outdir}/output-audio.aac -y");
        }

        /// <summary>
        /// Converts images into videos
        /// </summary>
        /// <param name="rate">target video framerate</param>
        /// <param name="resolution">target video resolution</param>
        /// <param name="srcdir">source images directory</param>
        public void ToVid(double rate, string resolution, string srcdir)
        {
            InternalCallFfmpeg($"-r {rate} -f image2 -s {resolution} -i {srcdir}/extracted_%0d.png -vcodec libx264 -crf 25 -pix_fmt yuv420p output.mp4 -y");
        }

        /// <summary>
        /// Insert audio into video
        /// </summary>
        /// <param name="input">input video path</param>
        /// <param name="outdir">video output directory</param>
        /// <param name="outfile"></param>
        public void InsertAudioTrack(string input, string outdir, string outfile)
        {
            InternalCallFfmpeg($"-i {input} -i {outdir}/output-audio.aac -codec copy -shortest {outfile} -y");
        }

        private void InternalCallFfmpeg(string argument)
        {
            var threadInfo = new ThreadStruct
            {
                FileName = _filePath,
                Argument = argument
            };

            _worker.StartWorker(threadInfo);
        }
    }
}
