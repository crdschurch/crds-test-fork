using System;
using System.Collections.Generic;

namespace Models
{
    public interface IAppSettings
    {
        Dictionary<string, string> Values();
    }
}
