using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Library.Core
{
    public class BaseClass : IDisposable
    {
        bool disposed;
        readonly SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
        
        public void Dispose()
        { 
            Dispose(true);
            GC.SuppressFinalize(this);           
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return; 
      
            if (disposing) {
                handle.Dispose();
            }
      
            disposed = true;
        }
    }
}
