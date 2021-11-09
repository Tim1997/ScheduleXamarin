using System;
using System.Collections.Generic;
using System.Text;

namespace BaseXamarin.Services.Awake
{
    public interface IActiveAware
    {
        bool IsActive { get; set; }
        event EventHandler IsActiveChanged;
    }
}
