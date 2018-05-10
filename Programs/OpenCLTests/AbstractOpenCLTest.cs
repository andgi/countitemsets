using System;

using Cloo;

namespace OpenCLTests
{
    public abstract class AbstractOpenCLTest
    {
        public AbstractOpenCLTest()
        {
            SetUpOpenCL();
        }

        public void SetUpOpenCL()
        {
            ComputePlatform platform = null;
            ComputeDevice gpu = null;
            Console.Out.WriteLine("OpenCL platforms:");
            foreach (ComputePlatform p in ComputePlatform.Platforms) {
                Console.Out.WriteLine("  " + p.Name + ", " + p.Profile + ", " + p.Vendor);
                foreach (ComputeDevice d in p.Devices) {
                    Console.Out.WriteLine("    " + d.Name + ", " + d.Type + ", " + d.MaxComputeUnits +
                                          ", " + d.OpenCLCVersionString + ", " + d.Available);
                    if (d.Type == ComputeDeviceTypes.Gpu) {
                        platform = p;
                        gpu = d;
                    }
                }
            }
            if (gpu != null) {
                computeContext = new ComputeContext(new ComputeDevice[] { gpu },
                                             new ComputeContextPropertyList(platform), null, IntPtr.Zero);
                computeCmdQueue = new ComputeCommandQueue(computeContext, gpu, ComputeCommandQueueFlags.None);
                Console.Out.WriteLine("Selected OpenCL platform: " + computeContext.Platform.Name + " and device: " + gpu.Name + ".");
            } else {
                computeContext = null;
                computeCmdQueue = null;
            }
            Console.Out.Flush();
        }

        protected ComputeContext computeContext;
        protected ComputeCommandQueue computeCmdQueue;
    }
}
