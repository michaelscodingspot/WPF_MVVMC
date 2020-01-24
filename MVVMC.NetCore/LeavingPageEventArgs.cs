using System;

namespace MVVMC
{
    public class LeavingPageEventArgs : EventArgs
    {
        public bool CancellingNavigationAllowed { get; set; }
        public bool CancelNavigation { get; set; } = false;
    }
}