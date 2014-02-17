using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DragonScale.Portable.Formatters;

namespace DragonScale.Windows.Formatters
{
    /// <summary>
    /// 
    /// </summary>
    public class FullJsonFormatterSettings : JsonFormatterSettings
    {
        /// <summary>
        /// Gets the default.
        /// </summary>
        /// <value>
        /// The default.
        /// </value>
        public static FullJsonFormatterSettings Default
        {
            get
            {
                var settings = new FullJsonFormatterSettings();
                settings.ContentProvider = new FullContentProvider(settings);
                return settings;
            }
        }

    }
}
