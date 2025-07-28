using UnityEngine;

namespace Watermelon
{
    [RegisterModule("Core/Multilanguage")]
    public class MultilanguageInitModule : InitModule
    {
        [SerializeField] MultilanguageSettings multilanguageSettings;
        public MultilanguageSettings MultilanguageSettings => multilanguageSettings;

        public override void CreateComponent(Initialiser Initialiser)
        {
            Multilanguage multilanguage = new Multilanguage();
            multilanguage.Init(multilanguageSettings);
        }

        public MultilanguageInitModule()
        {
            moduleName = "Multilanguage";
        }
    }
}

// -----------------
// Multilanguage v 1.0
// -----------------