using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DependencyInjectionTutorial.Models;

namespace DependencyInjectionTutorial.DataAccess
{
    public class RequestTrackerData
    {
        private List<string> _actions;

        public RequestTrackerData()
        {
            _actions = new List<string>();
        }

        public void Add(string action)
        {
            _actions.Add(action);
        }

        public string Display()
        {
            string result = "";
            foreach (string s in _actions)
            {
                result += s + ",";
            }

            result = result.Substring(0, result.Length-1);

            return result;
        }
    }
}
