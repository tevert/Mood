using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Mood.Startup))]

namespace Mood
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
