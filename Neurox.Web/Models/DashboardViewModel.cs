using Neurox.ConsoleGUI;
using Neurox.Web.Attributes;
using System;
using System.Collections.Generic;

namespace Neurox.Web.Models
{
    public class DashboardViewModel
    {
        [VueData("hiddenLayerWeights")]
        public double[,] HiddenLayerWeights { get; set; }
        [VueData("hiddenToOutputLayerWeights")]
        public double[,] HiddenToOutputLayerWeights { get; set; }
        [VueData("inputLayerModifiers")]
        public Dictionary<string, double> InputLayerModifiers { get; set; }

        [VueData("message")]
        public string Message { get; set; } = "Hello from Vue!";

        [VueData("menu")]
        public List<string> MenuItems { get; set; } = new List<string>()
        {
            "Menu 1",
            "Menu 2",
        };

        public string RazorMessage { get; set; } = "Hello from Razor!";

        // in a real app, this would be placed in the base view model class
        public Dictionary<string, object> VueData { get; set; } = new Dictionary<string, object>();
    }
}
