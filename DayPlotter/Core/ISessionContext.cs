using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace DayPlotter.Core
{
    public interface ISessionContext : INotifyPropertyChanged
    {
        //DispatcherTimer Timer { get; set; }
        //TimeSpan StartedFrom { get; set; }
        TimeSpan CurrnetTime { get; set; }
        //bool IsNormalTime { get; set; }
    }
}
