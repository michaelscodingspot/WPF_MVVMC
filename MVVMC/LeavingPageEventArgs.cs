using System;

namespace MVVMC
{
    public class LeavingPageEventArgs : EventArgs
    {
        public bool CancelNavigation { get; set; } = false;
    }
}