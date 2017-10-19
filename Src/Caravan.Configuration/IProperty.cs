using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Configuration
{
    public interface IProperty
    {
        string Key { get; }

        string Value { get; }

        PropertyConfig Config { get; }

        void Refresh();

        event EventHandler<PropertyChangedEventArgs> OnChange;
    }
}
