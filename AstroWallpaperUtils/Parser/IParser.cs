using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroWallpaperUtils.Parser
{
    public interface IParser
    {
        string ParserName { get; }

        Task<string> GetImageUrl();
    }
}
