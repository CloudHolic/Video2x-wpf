using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Video2x_wpf.Processor;
using Video2x_wpf.Utils;

namespace Video2x_wpf.Structures
{
    /// <summary>
    /// Communicates with waifu2x cui engine
    /// </summary>
    public static class Waifu2X
    {
        public enum Method
        {
            Cpu,
            Gpu,
            Cudnn
        }

        public enum Mode
        {
            [Description("noise")]Noise,
            [Description("scale")]Scale,
            [Description("noise_scale")]NoiseScale,
            [Description("auto_scale")]AutoScale
        }

        public static void Upscale(string waifuPath, Method method, string input, string output, int width, int height, int noise, Mode mode)
        {
            var worker = WorkerThread.Instance;
            var infos = new List<ThreadStruct>();
            var fileList = FileUtil.GetPngFiles(input);

            foreach (var cur in fileList)
            {
                if (cur == null)
                    continue;

                infos.Add(new ThreadStruct
                {
                    FileName = waifuPath,
                    Argument = $"-p {method.ToString().ToLower()} -i {cur} -o {Path.Combine(output, Path.GetFileName(cur))} -w {width} -h {height} -n {noise} -m {mode.GetDescription()} -y photo",
                    Index = fileList.IndexOf(cur)
                });
            }

            worker.StartWorkerPool(infos);
        }
    }
}
