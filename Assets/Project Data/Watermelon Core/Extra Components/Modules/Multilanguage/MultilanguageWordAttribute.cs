using UnityEngine;

namespace Watermelon
{
    public class MultilanguageWordAttribute : PropertyAttribute
    {
        private string filter;
        public string Filter
        {
            get { return filter; }
        }

        public MultilanguageWordAttribute(string filter = null)
        {
            this.filter = filter;
        }
    }
}

// -----------------
// Multilanguage v 1.0
// -----------------